using Gateway.Common.Models.Result;
using Gateway.ServiceRouting.Configuration;
using Gateway.ServiceRouting.Models;
using Microsoft.Extensions.Options;

namespace Gateway.ServiceRouting.Services;

/// <summary>
/// Default implementation of route resolver with dynamic service routing
/// </summary>
internal class ServiceRouteResolver(IOptionsMonitor<ServiceRoutingOptions> routingOptions) : IRouteResolver
{
    public Result<RouteMatch> ResolveRoute(string serviceId, string method, string downstreamPath)
    {
        var currentRoutingOptions = routingOptions.CurrentValue;

        // Find matching route configuration
        var routeConfig = currentRoutingOptions.Routes.FirstOrDefault(r =>
            r.ServiceId.Equals(serviceId, StringComparison.OrdinalIgnoreCase));

        if (routeConfig == null)
            return Result<RouteMatch>.Failure($"No route configuration found for service ID '{serviceId}'");

        // Check if method is allowed
        if (!IsMethodMatch(routeConfig.Methods, method))
            return Result<RouteMatch>.Failure($"Method '{method}' not allowed for service '{serviceId}'");

        var routeMatch = new RouteMatch(
            ServiceId: routeConfig.ServiceId,
            DownstreamPath: downstreamPath,
            AuthPolicy: routeConfig.AuthPolicy,
            RateLimitPolicy: routeConfig.RateLimitPolicy,
            CachePolicy: routeConfig.CachePolicy,
            LoadBalancingStrategy: routeConfig.LoadBalancingStrategy,
            Timeout: routeConfig.Timeout
        );

        return Result<RouteMatch>.Success(routeMatch);
    }

    private static bool IsMethodMatch(string[] routeMethods, string requestMethod)
    {
        return routeMethods.Length == 0 ||
               routeMethods.Contains(requestMethod, StringComparer.OrdinalIgnoreCase);
    }
}