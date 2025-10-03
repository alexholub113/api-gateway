using Gateway.Common.Configuration;
using Gateway.Common.Extensions;

namespace Gateway.RateLimiting.Middleware;

/// <summary>
/// Middleware for handling HTTP request rate limiting
/// </summary>
public class RateLimitingMiddleware(RequestDelegate next, IRateLimitService rateLimitService, IOptionsMonitor<GatewayOptions> gatewayOptions, ILogger<RateLimitingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        // Extract service ID from the request path or headers
        var serviceId = context.GetGatewayTargetServiceId();
        if (string.IsNullOrEmpty(serviceId))
        {
            await next(context);
            return;
        }

        // Resolve the target service settings
        var targetService = ResolveTargetService(serviceId);
        if (targetService == null || string.IsNullOrEmpty(targetService.RateLimitPolicy))
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

            await context.Response.WriteAsync("Rate limit exceeded");
            return;
        }

        // Rate limit passed - continue to next middleware
        await next(context);
    }

    private TargetServiceSettings? ResolveTargetService(string serviceId)
    {
        return gatewayOptions.CurrentValue.TargetServices.FirstOrDefault(r =>
            r.ServiceId.Equals(serviceId, StringComparison.OrdinalIgnoreCase));
    }
}