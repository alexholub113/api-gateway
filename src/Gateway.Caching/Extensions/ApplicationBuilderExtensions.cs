using Gateway.Caching.Middleware;
using Microsoft.AspNetCore.Builder;

namespace Gateway.Caching.Extensions;

/// <summary>
/// Extension methods for registering caching middleware
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds caching middleware to the application pipeline
    /// </summary>
    public static IApplicationBuilder UseCaching(this IApplicationBuilder app)
    {
        return app.UseMiddleware<CacheMiddleware>();
    }
}