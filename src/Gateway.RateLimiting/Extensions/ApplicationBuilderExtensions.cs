using Gateway.RateLimiting.Middleware;

namespace Gateway.RateLimiting.Extensions;

/// <summary>
/// Extension methods for registering rate limiting middleware
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds rate limiting middleware to the application pipeline
    /// </summary>
    public static IApplicationBuilder UseRateLimiting(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RateLimitingMiddleware>();
    }
}