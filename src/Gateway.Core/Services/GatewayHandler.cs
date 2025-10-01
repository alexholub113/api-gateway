using Gateway.Common.Configuration;
using Gateway.LoadBalancing;
using Gateway.Proxy;
using Gateway.RateLimiting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Gateway.Core.Services;

internal class GatewayHandler(
    IOptionsMonitor<Gateway.Common.Configuration.GatewayOptions> gatewayOptions,
    ILoadBalancer loadBalancer,
    IProxyHandler proxyHandler,
    IRateLimitService rateLimitService,
    ILogger<GatewayHandler> logger) : IGatewayHandler
{
    public async ValueTask<Result> RouteRequestAsync(HttpContext context, string serviceId, string downstreamPath)
    {
        return await ResolveRoute(serviceId)
            .Bind(targetServiceSettings => ApplyRateLimit(context, targetServiceSettings))
            .Bind(targetServiceSettings => SelectTargetInstance(targetServiceSettings, downstreamPath))
            .BindAsync(result => proxyHandler.ProxyRequestAsync(context, result.uri, downstreamPath));
    }

    private Result<TargetServiceSettings> ResolveRoute(string serviceId)
    {
        // Find matching route configuration
        var routeService = gatewayOptions.CurrentValue.TargetServices.FirstOrDefault(r =>
            r.ServiceId.Equals(serviceId, StringComparison.OrdinalIgnoreCase));

        if (routeService == null)
        {
            logger.LogWarning("No target service found for service ID '{ServiceId}'", serviceId);
            return Result<TargetServiceSettings>.Failure($"No target service found for service ID '{serviceId}'");
        }

        return Result<TargetServiceSettings>.Success(routeService);
    }

    private Result<TargetServiceSettings> ApplyRateLimit(HttpContext context, TargetServiceSettings targetServiceSettings)
    {
        // Skip rate limiting if no policy is configured
        if (string.IsNullOrEmpty(targetServiceSettings.RateLimitPolicy))
            return Result<TargetServiceSettings>.Success(targetServiceSettings);

        var result = rateLimitService.ApplyRateLimit(context, targetServiceSettings.RateLimitPolicy);

        return result.IsSuccess
            ? Result<TargetServiceSettings>.Success(targetServiceSettings)
            : Result<TargetServiceSettings>.Failure(result.Error);
    }

    private Result<(Uri uri, string downstreamPath)> SelectTargetInstance(TargetServiceSettings targetServiceSettings, string downstreamPath)
    {
        var instanceResult = loadBalancer.SelectInstance(targetServiceSettings.ServiceId);

        if (instanceResult.IsSuccess)
        {
            return Result<(Uri uri, string downstreamPath)>.Success((instanceResult.Value, downstreamPath));
        }
        else
        {
            logger.LogError("Failed to select instance for service '{ServiceId}': {Error}", targetServiceSettings.ServiceId, instanceResult.Error.Message);
            return Result<(Uri uri, string downstreamPath)>.Failure(instanceResult.Error);
        }
    }
}
