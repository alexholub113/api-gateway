using Gateway.LoadBalancing.Middleware;
using Microsoft.AspNetCore.Builder;

namespace Gateway.LoadBalancing.Extensions;

/// <summary>
/// Extension methods for configuring the load balancing middleware pipeline
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds load balancing middleware to the pipeline
    /// </summary>
    /// <param name="app">The application builder</param>
    /// <returns>The application builder for chaining</returns>
    public static IApplicationBuilder UseLoadBalancing(this IApplicationBuilder app)
    {
        return app.UseMiddleware<LoadBalancingMiddleware>();
    }
}