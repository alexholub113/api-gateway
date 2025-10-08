using Gateway.Metrics.Models;

namespace Gateway.Metrics;

/// <summary>
/// Public interface for retrieving gateway metrics from OpenTelemetry
/// </summary>
public interface IGatewayMetricsProvider
{
    /// <summary>
    /// Gets the current snapshot of all gateway metrics from OpenTelemetry meters
    /// </summary>
    GatewayMetricsSnapshot GetCurrentMetrics();
}
