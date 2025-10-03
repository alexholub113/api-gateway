using Gateway.Caching.Configuration;
using Gateway.Caching.Models;
using System.Security.Claims;

namespace Gateway.Caching.Services;

/// <summary>
/// In-memory cache service implementation
/// </summary>
internal class MemoryCacheService(
    IMemoryCache memoryCache,
    IOptions<CachingOptions> options,
    ILogger<MemoryCacheService> logger) : ICacheService
{
    private readonly CachingOptions _options = options.Value;
    private readonly ConcurrentDictionary<string, DateTime> _cacheKeys = new();

    public async ValueTask<Result<bool>> TryGetAndWriteAsync(HttpContext context, string serviceId, string policyName, CancellationToken cancellationToken = default)
    {
        var cacheResult = await TryGetAsync(context, serviceId, policyName);

        if (!cacheResult.IsSuccess)
        {
            logger.LogDebug("Cache check failed for service '{TargetServiceId}': {Error}", serviceId, cacheResult.Error.Message);
            return Result<bool>.Success(false);
        }

        if (cacheResult.Value?.IsHit == true && cacheResult.Value.Value != null)
        {
            logger.LogInformation("Cache hit for service '{TargetServiceId}', writing cached response", serviceId);

            // Write cached response directly to the HTTP context
            var response = cacheResult.Value.Value;
            context.Response.StatusCode = response.StatusCode;

            foreach (var header in response.Headers)
            {
                context.Response.Headers[header.Key] = header.Value;
            }

            if (response.Body?.Length > 0)
            {
                await context.Response.Body.WriteAsync(response.Body, cancellationToken);
            }

            return Result<bool>.Success(true);
        }

        logger.LogDebug("Cache miss for service '{TargetServiceId}'", serviceId);
        return Result<bool>.Success(false);
    }

    public ValueTask<Result> SetAsync(HttpContext context, string serviceId, string policyName, CachedResponse response, CancellationToken cancellationToken = default)
    {
        try
        {
            if (!_options.Policies.TryGetValue(policyName, out var policy))
            {
                logger.LogWarning("Cache policy '{PolicyName}' not found", policyName);
                return ValueTask.FromResult(Result.Failure($"Cache policy '{policyName}' not found"));
            }

            var cacheKey = BuildCacheKey(context, serviceId, policy);
            var keyString = cacheKey.ToKeyString();
            var expiresAt = DateTime.UtcNow.Add(policy.Duration);

            var cacheEntryOptions = new MemoryCacheEntryOptions
            {
                AbsoluteExpiration = expiresAt,
                Size = response.Body?.Length ?? 0
            };

            memoryCache.Set(keyString, response, cacheEntryOptions);
            _cacheKeys[keyString] = expiresAt;

            logger.LogDebug("Cached response for key '{CacheKey}' with expiration '{ExpiresAt}'", keyString, expiresAt);
            return ValueTask.FromResult(Result.Success());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to cache response for service '{TargetServiceId}'", serviceId);
            return ValueTask.FromResult(Result.Failure($"Failed to cache response: {ex.Message}"));
        }
    }

    public ValueTask<Result<bool>> IsCacheableAsync(HttpContext context, string? policyName, CancellationToken cancellationToken = default)
    {
        try
        {
            if (string.IsNullOrEmpty(policyName))
            {
                return ValueTask.FromResult(Result<bool>.Success(false));
            }

            if (!_options.EnableCaching)
            {
                return ValueTask.FromResult(Result<bool>.Success(false));
            }

            if (!_options.Policies.TryGetValue(policyName, out var policy))
            {
                logger.LogWarning("Cache policy '{PolicyName}' not found", policyName);
                return ValueTask.FromResult(Result<bool>.Success(false));
            }

            // Check if the HTTP method is cacheable
            var method = context.Request.Method;
            var isCacheable = policy.Methods?.Contains(method, StringComparer.OrdinalIgnoreCase) ?? false;

            logger.LogDebug("Request {Method} is {Status} for caching with policy '{PolicyName}'",
                method, isCacheable ? "eligible" : "not eligible", policyName);

            return ValueTask.FromResult(Result<bool>.Success(isCacheable));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to check if request is cacheable");
            return ValueTask.FromResult(Result<bool>.Failure($"Failed to check cacheability: {ex.Message}"));
        }
    }

    public ValueTask<Result<int>> ClearAsync(string pattern, CancellationToken cancellationToken = default)
    {
        try
        {
            var clearedCount = 0;
            var keysToRemove = new List<string>();

            foreach (var key in _cacheKeys.Keys)
            {
                if (string.IsNullOrEmpty(pattern) || key.Contains(pattern, StringComparison.OrdinalIgnoreCase))
                {
                    keysToRemove.Add(key);
                }
            }

            foreach (var key in keysToRemove)
            {
                memoryCache.Remove(key);
                _cacheKeys.TryRemove(key, out _);
                clearedCount++;
            }

            logger.LogInformation("Cleared {Count} cache entries matching pattern '{Pattern}'", clearedCount, pattern ?? "*");
            return ValueTask.FromResult(Result<int>.Success(clearedCount));
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to clear cache entries with pattern '{Pattern}'", pattern);
            return ValueTask.FromResult(Result<int>.Failure($"Failed to clear cache: {ex.Message}"));
        }
    }

    private ValueTask<Result<CacheResult<CachedResponse>>> TryGetAsync(HttpContext context, string serviceId, string policyName)
    {
        if (!_options.Policies.TryGetValue(policyName, out var policy))
        {
            logger.LogWarning("Cache policy '{PolicyName}' not found", policyName);
            return ValueTask.FromResult(Result<CacheResult<CachedResponse>>.Failure($"Cache policy '{policyName}' not found"));
        }

        var cacheKey = BuildCacheKey(context, serviceId, policy);
        var keyString = cacheKey.ToKeyString();

        if (memoryCache.TryGetValue(keyString, out var cachedValue) && cachedValue is CachedResponse response)
        {
            logger.LogDebug("Cache HIT for key '{CacheKey}'", keyString);

            var expiresAt = _cacheKeys.TryGetValue(keyString, out var expiry) ? expiry : DateTime.UtcNow.Add(policy.Duration);
            return ValueTask.FromResult(Result<CacheResult<CachedResponse>>.Success(
                CacheResult<CachedResponse>.Hit(response, expiresAt, keyString)));
        }

        logger.LogDebug("Cache MISS for key '{CacheKey}'", keyString);
        return ValueTask.FromResult(Result<CacheResult<CachedResponse>>.Success(
            CacheResult<CachedResponse>.Miss(keyString)));
    }

    private static CacheKey BuildCacheKey(HttpContext context, string serviceId, CachePolicy policy)
    {
        var cacheKey = new CacheKey
        {
            ServiceId = serviceId,
            Method = context.Request.Method,
            Path = context.Request.Path.Value ?? string.Empty,
            QueryString = policy.VaryByQuery ? context.Request.QueryString.Value ?? string.Empty : string.Empty
        };

        // Add vary-by headers
        if (policy.VaryByHeaders?.Length > 0)
        {
            var headers = new Dictionary<string, string>();
            foreach (var headerName in policy.VaryByHeaders)
            {
                if (context.Request.Headers.TryGetValue(headerName, out var headerValue))
                {
                    headers[headerName] = headerValue.ToString();
                }
            }
            cacheKey = cacheKey with { Headers = headers };
        }

        // Add user ID if policy requires it
        if (policy.VaryByUser)
        {
            var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                        context.User?.FindFirst("sub")?.Value ??
                        "anonymous";
            cacheKey = cacheKey with { UserId = userId };
        }

        return cacheKey;
    }
}