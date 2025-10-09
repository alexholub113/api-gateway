using Gateway.Caching.Models;
using Gateway.Common.Configuration;
using Gateway.Common.Extensions;

namespace Gateway.Caching.Middleware;

/// <summary>
/// Middleware for handling HTTP response caching
/// </summary>
public class CacheMiddleware(RequestDelegate next, ICacheService cacheService, IOptionsMonitor<GatewayOptions> gatewayOptions, ILogger<CacheMiddleware> logger)
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
        if (targetService?.CachePolicy == null)
        {
            await next(context);
            return;
        }

        // Check if request is cacheable
        var isCacheableResult = await cacheService.IsCacheableAsync(context, targetService.CachePolicy);
        if (!isCacheableResult.IsSuccess || !isCacheableResult.Value)
        {
            await next(context);
            return;
        }

        // Try to get cached response
        var cacheResult = await cacheService.TryGetAndWriteAsync(context, serviceId, targetService.CachePolicy);
        if (cacheResult.IsSuccess && cacheResult.Value)
        {
            // Cache hit - response has been written
            logger.LogInformation("Cache hit for service '{TargetServiceId}', served cached response", serviceId);
            return;
        }

        // Cache miss - capture and cache the response
        var originalBodyStream = context.Response.Body;
        using var responseBodyStream = new MemoryStream();
        context.Response.Body = responseBodyStream;

        try
        {
            await next(context);

            // Only cache successful responses
            if (context.Response.StatusCode >= 200 && context.Response.StatusCode < 300)
            {
                await CacheResponse(context, serviceId, targetService.CachePolicy, responseBodyStream);
            }
        }
        finally
        {
            // Copy the captured response back to the original stream
            responseBodyStream.Seek(0, SeekOrigin.Begin);
            await responseBodyStream.CopyToAsync(originalBodyStream);
            context.Response.Body = originalBodyStream;
        }
    }

    private TargetServiceSettings? ResolveTargetService(string serviceId)
    {
        return gatewayOptions.CurrentValue.TargetServices.FirstOrDefault(r =>
            r.ServiceId.Equals(serviceId, StringComparison.OrdinalIgnoreCase));
    }

    private async Task CacheResponse(HttpContext context, string serviceId, CachePolicy cachePolicy, MemoryStream responseBodyStream)
    {
        try
        {
            var responseBody = responseBodyStream.ToArray();

            var cachedResponse = new CachedResponse
            {
                StatusCode = context.Response.StatusCode,
                Headers = context.Response.Headers.ToDictionary(
                    h => h.Key,
                    h => h.Value.Where(v => v != null).Cast<string>().ToArray()),
                ContentType = context.Response.ContentType ?? string.Empty,
                Body = responseBody,
                CachedAt = DateTime.UtcNow
            };

            var cacheResult = await cacheService.SetAsync(context, serviceId, cachePolicy, cachedResponse);
            if (cacheResult.IsSuccess)
            {
                logger.LogDebug("Successfully cached response for service '{TargetServiceId}', size: {Size} bytes",
                    serviceId, responseBody.Length);
            }
            else
            {
                logger.LogWarning("Failed to cache response for service '{TargetServiceId}': {Error}",
                    serviceId, cacheResult.Error.Message);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error occurred while caching response for service '{TargetServiceId}'", serviceId);
        }
    }
}