using Gateway.Core.Abstractions;
using Gateway.LoadBalancing.Abstractions;
using Gateway.LoadBalancing.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Gateway.LoadBalancing.Middleware;

/// <summary>
/// Middleware that selects a service instance for load balancing
/// </summary>
internal class LoadBalancingMiddleware(RequestDelegate next, ILoadBalancer loadBalancer, IOptionsMonitor<LoadBalancingOptions> options)
{
    public async Task InvokeAsync(HttpContext context, IGatewayContext gatewayContext)
    {
        // Only process if we have a route match but no selected instance yet
        if (gatewayContext.RouteMatch != null && gatewayContext.SelectedInstance == null)
        {
            var loadBalancingOptions = options.CurrentValue;
            var strategy = gatewayContext.RouteMatch.LoadBalancingStrategy ?? loadBalancingOptions.DefaultStrategy;
            
            var selectedInstance = await loadBalancer.SelectInstanceAsync(
                gatewayContext.RouteMatch.TargetServiceName, 
                strategy);
            
            if (selectedInstance != null)
            {
                gatewayContext.SelectedInstance = selectedInstance;
            }
            else
            {
                // No healthy instances available
                context.Response.StatusCode = 503; // Service Unavailable
                await context.Response.WriteAsync($"No healthy instances available for service '{gatewayContext.RouteMatch.TargetServiceName}'");
                return;
            }
        }
        
        await next(context);
    }
}