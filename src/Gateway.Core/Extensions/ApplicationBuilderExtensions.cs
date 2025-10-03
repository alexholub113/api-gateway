using Gateway.Caching.Extensions;
using Microsoft.AspNetCore.Builder;

namespace Gateway.Core.Extensions;

/// <summary>
/// Extension methods for registering caching middleware
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds gateway middlewares to the application pipeline
    /// </summary>
    public static IApplicationBuilder UseGateway(this IApplicationBuilder app)
    {
        return app.UseCaching();
    }
}