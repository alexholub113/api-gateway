using Gateway.Core.Abstractions;
using Gateway.Core.Configuration;
using Gateway.Routing.Abstractions;
using Gateway.Routing.Configuration;
using Microsoft.Extensions.Options;
using System.Text.RegularExpressions;

namespace Gateway.Routing.Services;

/// <summary>
/// Default implementation of route resolver
/// </summary>
internal class RouteResolver(
    IOptionsMonitor<RoutingOptions> routingOptions,
    IOptionsMonitor<ServicesOptions> servicesOptions) : IRouteResolver
{
    private readonly RoutingOptions _routingOptions = routingOptions.CurrentValue;
    private readonly ServicesOptions _servicesOptions = servicesOptions.CurrentValue;

    public Result<RouteMatch> ResolveRoute(string path, string method)
    {
        foreach (var route in _routingOptions.Routes)
        {
            if (!IsMethodMatch(route.Methods, method))
                continue;

            if (!IsPathMatch(route.Path, path))
                continue;

            var service = _servicesOptions.Services.FirstOrDefault(s => s.Name == route.TargetService);
            if (service == null)
                return Result<RouteMatch>.Failure($"Service '{route.TargetService}' not found");

            var routeMatch = new RouteMatch(
                RouteId: route.Id,
                Pattern: route.Path,
                TargetServiceName: route.TargetService,
                AuthPolicy: route.AuthPolicy,
                RateLimitPolicy: route.RateLimitPolicy,
                CachePolicy: route.CachePolicy,
                LoadBalancingStrategy: route.LoadBalancingStrategy,
                Timeout: route.Timeout
            );
            return Result<RouteMatch>.Success(routeMatch);
        }

        return Result<RouteMatch>.Failure("No matching route found");
    }

    private static bool IsMethodMatch(string[] routeMethods, string requestMethod)
    {
        return routeMethods.Length == 0 || 
               routeMethods.Contains(requestMethod, StringComparer.OrdinalIgnoreCase);
    }

    private static bool IsPathMatch(string routePattern, string requestPath)
    {
        // Convert simple wildcard pattern to regex
        // /api/users/* becomes ^/api/users/.*$
        var regexPattern = "^" + Regex.Escape(routePattern).Replace("\\*", ".*") + "$";
        return Regex.IsMatch(requestPath, regexPattern, RegexOptions.IgnoreCase);
    }
}