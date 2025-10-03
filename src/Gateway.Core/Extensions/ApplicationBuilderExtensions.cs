using Gateway.Caching.Extensions;
using Gateway.RateLimiting.Extensions;
using Microsoft.AspNetCore.Builder;

namespace Gateway.Core.Extensions;

/// <summary>
/// Extension methods for registering gateway middlewares
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds gateway middlewares to the application pipeline in the correct order
    /// </summary>
    public static IApplicationBuilder UseGateway(this IApplicationBuilder app)
    {
        // Order is important: Rate limiting should come before caching
        // to prevent serving cached responses to rate-limited clients
        return app
            .UseRateLimiting()
            .UseCaching();
    }
}