using System.Diagnostics.Metrics;

namespace Gateway.Proxy.Telemetry;

/// <summary>
/// Telemetry for proxy operations
/// </summary>
public sealed class ProxyTelemetry : IDisposable
{
    private readonly Meter _meter;

    // Counters
    private readonly Counter<long> _backendRequestsTotal;
    private readonly Counter<long> _backendErrorsTotal;
    private readonly Counter<long> _healthChecksTotal;

    // Histograms  
    private readonly Histogram<double> _proxyDuration;
    private readonly Histogram<double> _backendDuration;
    private readonly Histogram<double> _healthCheckDuration;

    public ProxyTelemetry()
    {
        _meter = new Meter("Gateway.Proxy", "1.0.0");

        _backendRequestsTotal = _meter.CreateCounter<long>(
            "gateway_backend_requests_total",
            "requests",
            "Backend requests by service and instance");

        _backendErrorsTotal = _meter.CreateCounter<long>(
            "gateway_backend_errors_total",
            "errors",
            "Backend errors by service and type");

        _healthChecksTotal = _meter.CreateCounter<long>(
            "gateway_health_checks_total",
            "checks",
            "Health checks by result and service");

        _proxyDuration = _meter.CreateHistogram<double>(
            "gateway_proxy_duration",
            "milliseconds",
            "Time spent proxying to backend");

        _backendDuration = _meter.CreateHistogram<double>(
            "gateway_backend_duration",
            "milliseconds",
            "Backend response time");

        _healthCheckDuration = _meter.CreateHistogram<double>(
            "gateway_health_check_duration",
            "milliseconds",
            "Health check duration");
    }

    /// <summary>
    /// Records backend request
    /// </summary>
    public void RecordBackendRequest(string serviceId, string instanceId, string method, int statusCode, double durationMs)
    {
        var tags = new KeyValuePair<string, object?>[]
        {
            new("service_id", serviceId),
            new("instance_id", instanceId),
            new("method", method),
            new("status_code", statusCode.ToString())
        };

        _backendRequestsTotal.Add(1, tags);
        _backendDuration.Record(durationMs, tags);
    }

    /// <summary>
    /// Records proxy operation duration
    /// </summary>
    public void RecordProxyDuration(string serviceId, string instanceId, double durationMs)
    {
        var tags = new KeyValuePair<string, object?>[]
        {
            new("service_id", serviceId),
            new("instance_id", instanceId)
        };

        _proxyDuration.Record(durationMs, tags);
    }

    /// <summary>
    /// Records backend error
    /// </summary>
    public void RecordBackendError(string serviceId, string instanceId, string errorType)
    {
        var tags = new KeyValuePair<string, object?>[]
        {
            new("service_id", serviceId),
            new("instance_id", instanceId),
            new("error_type", errorType)
        };

        _backendErrorsTotal.Add(1, tags);
    }

    /// <summary>
    /// Records health check
    /// </summary>
    public void RecordHealthCheck(string serviceId, string instanceId, bool isHealthy, double durationMs)
    {
        var result = isHealthy ? "healthy" : "unhealthy";

        var tags = new KeyValuePair<string, object?>[]
        {
            new("service_id", serviceId),
            new("instance_id", instanceId),
            new("result", result)
        };

        _healthChecksTotal.Add(1, tags);
        _healthCheckDuration.Record(durationMs, tags);
    }

    public void Dispose()
    {
        _meter.Dispose();
    }
}