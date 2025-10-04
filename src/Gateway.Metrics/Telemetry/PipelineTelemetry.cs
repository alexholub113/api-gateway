using System.Diagnostics.Metrics;

namespace Gateway.Metrics.Telemetry;

/// <summary>
/// Telemetry for gateway pipeline operations
/// </summary>
public sealed class PipelineTelemetry : IDisposable
{
    private readonly Meter _meter;

    // Histograms for pipeline stage durations
    private readonly Histogram<double> _routeResolutionDuration;
    private readonly Histogram<double> _pipelineStageDuration;

    public PipelineTelemetry()
    {
        _meter = new Meter("Gateway.Metrics", "1.0.0");

        _routeResolutionDuration = _meter.CreateHistogram<double>(
            "gateway_route_resolution_duration",
            "milliseconds",
            "Time to resolve routes");

        _pipelineStageDuration = _meter.CreateHistogram<double>(
            "gateway_pipeline_stage_duration",
            "milliseconds",
            "Duration by pipeline stage");
    }

    /// <summary>
    /// Records route resolution duration
    /// </summary>
    public void RecordRouteResolution(double durationMs, string? route = null)
    {
        var tags = new KeyValuePair<string, object?>[]
        {
            new("route", route ?? "unknown")
        };

        _routeResolutionDuration.Record(durationMs, tags);
        _pipelineStageDuration.Record(durationMs, new KeyValuePair<string, object?>[]
        {
            new("stage", "route_resolution"),
            new("route", route ?? "unknown")
        });
    }

    /// <summary>
    /// Records pipeline stage duration
    /// </summary>
    public void RecordPipelineStage(string stage, double durationMs, string? serviceId = null)
    {
        var tags = new KeyValuePair<string, object?>[]
        {
            new("stage", stage),
            new("service_id", serviceId ?? "unknown")
        };

        _pipelineStageDuration.Record(durationMs, tags);
    }

    public void Dispose()
    {
        _meter.Dispose();
    }
}