using Gateway.RateLimiting.Configuration;
using Gateway.RateLimiting.Models;

namespace Gateway.RateLimiting.Services;

internal class RateLimitService(IOptionsMonitor<RateLimitingOptions> options, ILogger<RateLimitService> logger) : IRateLimitService
{
    private readonly ConcurrentDictionary<string, ClientRateLimitState> _clientStates = new();
    private readonly SemaphoreSlim _cleanupSemaphore = new(1, 1);
    private DateTime _lastCleanup = DateTime.UtcNow;

    public Result<RateLimitResult> ApplyRateLimit(HttpContext context, string policyName)
    {
        if (!options.CurrentValue.Policies.TryGetValue(policyName, out var policy))
        {
            logger.LogWarning("Rate limit policy '{policyName}' not found", policyName);
            return Result<RateLimitResult>.Failure(Error.NotFound($"Rate limit policy '{policyName}' not found"));
        }

        var clientKey = ExtractClientKey(context);
        var rateLimitResult = IsRequestAllowedAsync(clientKey, policy);

        if (rateLimitResult.IsFailure)
            return Result<RateLimitResult>.Failure(rateLimitResult.Error);

        var result = rateLimitResult.Value;

        // Set rate limit headers
        context.Response.Headers["X-RateLimit-Remaining"] = result.RemainingRequests.ToString();

        return Result<RateLimitResult>.Success(result);
    }

    private Result<RateLimitResult> IsRequestAllowedAsync(string clientKey, RateLimitPolicy policy)
    {
        var now = DateTime.UtcNow;
        var key = $"{clientKey}:{policy.GetHashCode()}";

        var clientState = _clientStates.GetOrAdd(key, _ => new ClientRateLimitState());

        lock (clientState.Lock)
        {
            var result = policy.Algorithm switch
            {
                RateLimitAlgorithm.SlidingWindow => CheckSlidingWindow(clientState, policy, now),
                RateLimitAlgorithm.TokenBucket => CheckTokenBucket(clientState, policy, now),
                RateLimitAlgorithm.FixedWindow => CheckFixedWindow(clientState, policy, now),
                _ => throw new NotSupportedException($"Rate limit algorithm '{policy.Algorithm}' is not supported")
            };

            if (result.IsAllowed)
            {
                logger.LogDebug("Request allowed for client '{clientKey}', remaining: {RemainingRequests}",
                    clientKey, result.RemainingRequests);
            }
            else
            {
                logger.LogInformation("Rate limit exceeded for client '{clientKey}', retry after: {RetryAfter}",
                    clientKey, result.RetryAfter);
            }

            if (DateTime.UtcNow - _lastCleanup >= TimeSpan.FromMinutes(5))
            {
                // Cleanup old states if needed
                _ = Task.Run(CleanupIfNeededAsync);
            }

            return Result<RateLimitResult>.Success(result);
        }
    }

    private static string ExtractClientKey(HttpContext context)
    {
        // Try to get the real IP from headers (for when behind proxy/load balancer)
        var forwardedFor = context.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrEmpty(forwardedFor))
        {
            var firstIp = forwardedFor.Split(',')[0].Trim();
            if (System.Net.IPAddress.TryParse(firstIp, out _))
                return firstIp;
        }

        var realIp = context.Request.Headers["X-Real-IP"].FirstOrDefault();
        if (!string.IsNullOrEmpty(realIp) && System.Net.IPAddress.TryParse(realIp, out _))
            return realIp;

        // Fall back to connection remote IP
        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }

    private static RateLimitResult CheckSlidingWindow(ClientRateLimitState state, RateLimitPolicy policy, DateTime now)
    {
        // Remove old requests outside the window
        var windowStart = now - policy.WindowSize;
        state.RequestTimestamps.RemoveAll(t => t < windowStart);

        if (state.RequestTimestamps.Count >= policy.RequestsPerWindow)
        {
            var oldestRequest = state.RequestTimestamps.Min();
            var retryAfter = oldestRequest.Add(policy.WindowSize) - now;
            return new RateLimitResult(false, 0, retryAfter);
        }

        state.RequestTimestamps.Add(now);
        var remaining = policy.RequestsPerWindow - state.RequestTimestamps.Count;
        return new RateLimitResult(true, remaining, TimeSpan.Zero);
    }

    private static RateLimitResult CheckTokenBucket(ClientRateLimitState state, RateLimitPolicy policy, DateTime now)
    {
        if (state.LastRefill == DateTime.MinValue)
        {
            state.LastRefill = now;
            state.TokenCount = policy.RequestsPerWindow;
        }

        // Refill tokens based on elapsed time
        var elapsed = now - state.LastRefill;
        var tokensToAdd = (int)(elapsed.TotalSeconds * policy.RequestsPerWindow / policy.WindowSize.TotalSeconds);

        if (tokensToAdd > 0)
        {
            state.TokenCount = Math.Min(policy.RequestsPerWindow, state.TokenCount + tokensToAdd);
            state.LastRefill = now;
        }

        if (state.TokenCount <= 0)
        {
            var retryAfter = TimeSpan.FromSeconds(policy.WindowSize.TotalSeconds / policy.RequestsPerWindow);
            return new RateLimitResult(false, 0, retryAfter);
        }

        state.TokenCount--;
        return new RateLimitResult(true, state.TokenCount, TimeSpan.Zero);
    }

    private static RateLimitResult CheckFixedWindow(ClientRateLimitState state, RateLimitPolicy policy, DateTime now)
    {
        var windowStart = new DateTime(now.Ticks / policy.WindowSize.Ticks * policy.WindowSize.Ticks);

        if (state.WindowStart != windowStart)
        {
            state.WindowStart = windowStart;
            state.RequestCount = 0;
        }

        if (state.RequestCount >= policy.RequestsPerWindow)
        {
            var retryAfter = windowStart.Add(policy.WindowSize) - now;
            return new RateLimitResult(false, 0, retryAfter);
        }

        state.RequestCount++;
        var remaining = policy.RequestsPerWindow - state.RequestCount;
        return new RateLimitResult(true, remaining, TimeSpan.Zero);
    }

    // Cleanup old client states periodically
    private async Task CleanupIfNeededAsync()
    {
        if (DateTime.UtcNow - _lastCleanup < TimeSpan.FromMinutes(5))
            return;

        if (await _cleanupSemaphore.WaitAsync(100))
        {
            if (DateTime.UtcNow - _lastCleanup < TimeSpan.FromMinutes(5))
                return;

            try
            {
                var cutoff = DateTime.UtcNow.AddHours(-1);
                var keysToRemove = _clientStates
                    .Where(kvp => kvp.Value.LastAccess < cutoff)
                    .Select(kvp => kvp.Key)
                    .ToList();

                foreach (var key in keysToRemove)
                {
                    _clientStates.TryRemove(key, out _);
                }

                _lastCleanup = DateTime.UtcNow;
                logger.LogDebug("Cleaned up {Count} old rate limit states", keysToRemove.Count);
            }
            finally
            {
                _cleanupSemaphore.Release();
            }
        }
    }

    private class ClientRateLimitState
    {
        public readonly object Lock = new();
        public List<DateTime> RequestTimestamps { get; set; } = [];
        public DateTime LastRefill { get; set; } = DateTime.MinValue;
        public int TokenCount { get; set; }
        public DateTime WindowStart { get; set; } = DateTime.MinValue;
        public int RequestCount { get; set; }
        public DateTime LastAccess { get; set; } = DateTime.UtcNow;
    }
}