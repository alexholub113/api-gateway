using Gateway.Auth.Extensions;
using Gateway.Caching.Extensions;
using Gateway.Metrics.Extensions;
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
        return app
            .UseCoreMetrics()
            .UseGatewayTelemetry()
            .UseRateLimiting()
            .UseGatewayAuth()
            .UseCaching();
    }
}