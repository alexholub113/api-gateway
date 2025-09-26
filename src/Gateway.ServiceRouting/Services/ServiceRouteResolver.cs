using Gateway.Core.Abstractions;
using Gateway.Core.Configuration;
using Gateway.ServiceRouting.Abstractions;
using Gateway.ServiceRouting.Configuration;
using Microsoft.Extensions.Options;

namespace Gateway.ServiceRouting.Services;

/// <summary>
/// Default implementation of route resolver with dynamic service routing
/// </summary>
internal class ServiceRouteResolver(
    IOptionsMonitor<ServiceRoutingOptions> routingOptions,
    IOptionsMonitor<ServicesOptions> servicesOptions) : IRouteResolver
{
    public Result<RouteMatch> ResolveRoute(string path, string method)
    {
        var currentRoutingOptions = routingOptions.CurrentValue;
        var currentServicesOptions = servicesOptions.CurrentValue;
        // Check if path matches the dynamic routing pattern: /{RoutePrefix}/{serviceId}/**
        var routePrefix = currentRoutingOptions.RoutePrefix.Trim('/');
        var expectedPrefix = $"/{routePrefix}/";

        if (!path.StartsWith(expectedPrefix, StringComparison.OrdinalIgnoreCase))
            return Result<RouteMatch>.Failure($"Path does not start with expected routing prefix '/{routePrefix}/'");

        // Extract service ID from path: /route/{serviceId}/remaining/path
        var pathAfterPrefix = path[expectedPrefix.Length..];
        var segments = pathAfterPrefix.Split('/', StringSplitOptions.RemoveEmptyEntries);

        if (segments.Length == 0)
            return Result<RouteMatch>.Failure("No service ID found in path");

        var serviceId = segments[0];

        // Find matching route configuration
        var routeConfig = currentRoutingOptions.Routes.FirstOrDefault(r =>
            r.ServiceId.Equals(serviceId, StringComparison.OrdinalIgnoreCase));

        if (routeConfig == null)
            return Result<RouteMatch>.Failure($"No route configuration found for service ID '{serviceId}'");

        // Check if method is allowed
        if (!IsMethodMatch(routeConfig.Methods, method))
            return Result<RouteMatch>.Failure($"Method '{method}' not allowed for service '{serviceId}'");

        // Verify target service exists
        var service = currentServicesOptions.Services.FirstOrDefault(s => s.Name == routeConfig.TargetService);
        if (service == null)
            return Result<RouteMatch>.Failure($"Target service '{routeConfig.TargetService}' not found");

        var routeMatch = new RouteMatch(
            RouteId: routeConfig.ServiceId,
            Pattern: $"/{routePrefix}/{routeConfig.ServiceId}/*",
            TargetServiceName: routeConfig.TargetService,
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