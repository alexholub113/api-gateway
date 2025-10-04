using Gateway.Metrics.Telemetry;
using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Metrics;
using OpenTelemetry.Trace;

namespace Gateway.Metrics.Extensions;

/// <summary>
/// Extension methods for configuring OpenTelemetry in the Gateway
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds OpenTelemetry configuration for the Gateway
    /// </summary>
    public static IServiceCollection AddGatewayTelemetry(this IServiceCollection services)
    {
        // Register pipeline telemetry service (Gateway.Metrics owns this)
        services.AddSingleton<PipelineTelemetry>();

        services.AddOpenTelemetry()
            .WithMetrics(metrics =>
            {
                metrics
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddMeter("Gateway.RateLimiting")    // Rate limiting metrics
                    .AddMeter("Gateway.Caching")         // Caching metrics
                    .AddMeter("Gateway.LoadBalancing")   // Load balancing metrics  
                    .AddMeter("Gateway.Proxy")           // Proxy metrics
                    .AddMeter("Gateway.Metrics")         // Pipeline metrics
                    .AddConsoleExporter()
                    .AddPrometheusExporter(); // Expose metrics at /metrics endpoint
            })
            .WithTracing(tracing =>
            {
                tracing
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation()
                    .AddConsoleExporter();
            });

        return services;
    }
}