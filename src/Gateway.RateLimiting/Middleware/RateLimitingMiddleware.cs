using Gateway.Common.Configuration;
using Gateway.Common.Extensions;
using Gateway.RateLimiting.Telemetry;
using System.Diagnostics;

namespace Gateway.RateLimiting.Middleware;

/// <summary>
/// Middleware for handling HTTP request rate limiting
/// </summary>
public class RateLimitingMiddleware(RequestDelegate next, IRateLimitService rateLimitService, IOptionsMonitor<GatewayOptions> gatewayOptions, RateLimitingTelemetry telemetry, ILogger<RateLimitingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var serviceId = string.Empty;

        try
        {
            // Extract service ID from the request path or headers
            serviceId = context.GetGatewayTargetServiceId();
            if (string.IsNullOrEmpty(serviceId))
            {
                await next(context);
                return;
            }

            // Resolve the target service settings
            var targetService = ResolveTargetService(serviceId);
            if (targetService?.RateLimitPolicy == null)
            {
                await next(context);
                return;
            }

            // Apply rate limiting
            var rateLimitResult = rateLimitService.ApplyRateLimit(context, targetService.RateLimitPolicy);
            if (rateLimitResult.IsSuccess && !rateLimitResult.Value.IsAllowed)
            {
                // Rate limit exceeded - return 429 Too Many Requests
                logger.LogWarning("Rate limit exceeded for service '{ServiceId}'", serviceId);

                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.Response.Headers["X-RateLimit-Retry-After"] = ((int)rateLimitResult.Value.RetryAfter.TotalSeconds).ToString();

                // Record rate limit violation
                telemetry.RecordRateLimitViolation(serviceId, targetService.RateLimitPolicy.Algorithm.ToString(), GetClientId(context));

                await context.Response.WriteAsync("Rate limit exceeded");
                return;
            }

            // Record successful rate limit check
            if (rateLimitResult.IsSuccess)
            {
                telemetry.RecordRateLimitDecision(serviceId, targetService.RateLimitPolicy.Algorithm.ToString(), rateLimitResult.Value.IsAllowed, GetClientId(context));
            }

            // Rate limit passed - continue to next middleware
            await next(context);
        }
        finally
        {
            // Record request metrics
            stopwatch.Stop();
            if (!string.IsNullOrEmpty(serviceId))
            {
                telemetry.RecordRequest(
                    serviceId,
                    context.Request.Method,
                    context.Response.StatusCode,
                    stopwatch.Elapsed.TotalMilliseconds);
            }
        }
    }

    private TargetServiceSettings? ResolveTargetService(string serviceId)
    {
        return gatewayOptions.CurrentValue.TargetServices.FirstOrDefault(r =>
            r.ServiceId.Equals(serviceId, StringComparison.OrdinalIgnoreCase));
    }

    private string GetClientId(HttpContext context)
    {
        // Try to get client ID from headers first
        if (context.Request.Headers.TryGetValue("X-Client-Id", out var clientIdHeader))
        {
            return clientIdHeader.ToString();
        }

        // Fall back to IP address
        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}