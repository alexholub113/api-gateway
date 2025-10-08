using System.Diagnostics.Metrics;

namespace Gateway.Metrics.Telemetry;

/// <summary>
/// Telemetry for gateway core operations
/// Records ALL requests passing through the gateway regardless of module configuration
/// </summary>
public sealed class CoreTelemetry : IDisposable
{
    private readonly Meter _meter;

    // Counters
    private readonly Counter<long> _requestsTotal;
    private readonly Counter<long> _errorsTotal;

    // Histograms
    private readonly Histogram<double> _requestDuration;
    private readonly Histogram<double> _routeResolutionDuration;

    public CoreTelemetry()
    {
        _meter = new Meter("Gateway.Core", "1.0.0");

        // Initialize counters
        _requestsTotal = _meter.CreateCounter<long>(
            "gateway_requests_total",
            "requests",
            "Total number of requests processed by the gateway");

        _errorsTotal = _meter.CreateCounter<long>(
            "gateway_errors_total",
            "errors",
            "Total number of errors by type");

        // Initialize histograms
        _requestDuration = _meter.CreateHistogram<double>(
            "gateway_request_duration",
            "milliseconds",
            "Request processing duration");

        _routeResolutionDuration = _meter.CreateHistogram<double>(
            "gateway_route_resolution_duration",
            "milliseconds",
            "Time to resolve routes");
    }

    /// <summary>
    /// Records a complete request with its basic metrics
    /// This is the PRIMARY metric for counting ALL gateway requests
    /// </summary>
    public void RecordRequest(string serviceId, string method, int statusCode, double durationMs)
    {
        var tags = new KeyValuePair<string, object?>[]
        {
            new("service_id", serviceId),
            new("method", method),
            new("status_code", statusCode.ToString())
        };

        _requestsTotal.Add(1, tags);
        _requestDuration.Record(durationMs, tags);

        // Record errors for non-2xx status codes
        if (statusCode >= 400)
        {
            var errorType = statusCode switch
            {
                >= 400 and < 500 => "4xx",
                >= 500 => "5xx",
                _ => "unknown"
            };

            var errorTags = new KeyValuePair<string, object?>[]
            {
                new("service_id", serviceId),
                new("error_type", errorType),
                new("status_code", statusCode.ToString())
            };

            _errorsTotal.Add(1, errorTags);
        }
    }

    /// <summary>
    /// Records route resolution duration
    /// </summary>
    public void RecordRouteResolution(double durationMs, string? serviceId = null)
    {
        var tags = new KeyValuePair<string, object?>[]
        {
            new("service_id", serviceId ?? "unknown")
        };

        _routeResolutionDuration.Record(durationMs, tags);
    }

    public void Dispose()
    {
        _meter.Dispose();
    }
}
