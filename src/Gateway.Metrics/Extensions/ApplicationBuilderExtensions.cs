using Gateway.Metrics.Middleware;
using Microsoft.AspNetCore.Builder;

namespace Gateway.Metrics.Extensions;

/// <summary>
/// Extension methods for configuring telemetry middleware and endpoints
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Maps the Prometheus metrics scraping endpoint
    /// </summary>
    public static IApplicationBuilder UseGatewayTelemetry(this IApplicationBuilder app)
    {
        // Map Prometheus metrics endpoint
        app.UseRouting();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapPrometheusScrapingEndpoint();
        });

        return app;
    }

    /// <summary>
    /// Adds core telemetry middleware to record ALL gateway requests
    /// </summary>
    public static IApplicationBuilder UseCoreMetrics(this IApplicationBuilder app)
    {
        return app.UseMiddleware<CoreTelemetryMiddleware>();
    }
}