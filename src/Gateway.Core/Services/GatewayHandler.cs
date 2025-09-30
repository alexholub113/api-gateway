using Gateway.Core.Configuration;
using Gateway.LoadBalancing;
using Gateway.Proxy;
using Microsoft.Extensions.Options;

namespace Gateway.Core.Services;

internal class GatewayHandler(IOptionsMonitor<GatewayOptions> gatewayOptions, ILoadBalancer loadBalancer, IProxyHandler proxyHandler) : IGatewayHandler
{
    public async ValueTask<Result> RouteRequestAsync(HttpContext context, string serviceId, string downstreamPath)
    {
        return await GetTargetService(serviceId)
            .Bind(serviceSettings => loadBalancer.SelectInstance(serviceSettings.ServiceId)
            .Map(uri => (serviceSettings, uri)))
            .BindAsync(results => proxyHandler.ProxyRequestAsync(context, results.uri, downstreamPath));
    }

    private Result<TargetServiceSettings> GetTargetService(string serviceId)
    {
        // Find matching route configuration
        var routeService = gatewayOptions.CurrentValue.TargetServices.FirstOrDefault(r =>
            r.ServiceId.Equals(serviceId, StringComparison.OrdinalIgnoreCase));

        if (routeService == null)
            return Result<TargetServiceSettings>.Failure($"No target service found for service ID '{serviceId}'");

        return Result<TargetServiceSettings>.Success(routeService);
    }
}
