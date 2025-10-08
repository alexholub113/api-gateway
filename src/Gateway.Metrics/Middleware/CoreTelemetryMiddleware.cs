using Gateway.Common.Extensions;
using Gateway.Metrics.Telemetry;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Gateway.Metrics.Middleware;

/// <summary>
/// Middleware that records telemetry for ALL gateway requests
/// This runs at the core level regardless of other module configurations
/// </summary>
public class CoreTelemetryMiddleware(RequestDelegate next, CoreTelemetry telemetry, ILogger<CoreTelemetryMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        var serviceId = string.Empty;

        try
        {
            // Extract service ID from the request
            serviceId = context.GetGatewayTargetServiceId();
            
            // Continue to next middleware
            await next(context);
        }
        finally
        {
            // Record request metrics
            stopwatch.Stop();
            
            // Only record if we have a service ID (means it's a gateway request)
            if (!string.IsNullOrEmpty(serviceId))
            {
                telemetry.RecordRequest(
                    serviceId,
                    context.Request.Method,
                    context.Response.StatusCode,
                    stopwatch.Elapsed.TotalMilliseconds);

                logger.LogDebug(
                    "Gateway request completed: {ServiceId} {Method} {StatusCode} {Duration}ms",
                    serviceId,
                    context.Request.Method,
                    context.Response.StatusCode,
                    stopwatch.Elapsed.TotalMilliseconds);
            }
        }
    }
}
