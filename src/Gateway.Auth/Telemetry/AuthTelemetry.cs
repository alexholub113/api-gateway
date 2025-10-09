using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace Gateway.Auth.Telemetry;

internal class AuthTelemetry
{
    private readonly Counter<long> _authRequestsCounter;
    private readonly Histogram<double> _authDurationHistogram;
    private readonly Counter<long> _authErrorsCounter;

    public AuthTelemetry(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create("Gateway.Auth");

        _authRequestsCounter = meter.CreateCounter<long>(
            "gateway_auth_requests_total",
            unit: "requests",
            description: "Total number of authentication attempts");

        _authDurationHistogram = meter.CreateHistogram<double>(
            "gateway_auth_duration",
            unit: "ms",
            description: "Authentication validation duration");

        _authErrorsCounter = meter.CreateCounter<long>(
            "gateway_auth_errors_total",
            unit: "errors",
            description: "Total number of authentication errors");
    }

    public void RecordAuthRequest(string serviceId, string result, double durationMs, string? policy = null)
    {
        var tags = new TagList
        {
            { "service", serviceId },
            { "result", result }
        };

        if (!string.IsNullOrEmpty(policy))
        {
            tags.Add("policy", policy);
        }

        _authRequestsCounter.Add(1, tags);
        _authDurationHistogram.Record(durationMs, tags);
    }

    public void RecordAuthError(string serviceId, string errorType, string? policy = null)
    {
        var tags = new TagList
        {
            { "service", serviceId },
            { "error_type", errorType }
        };

        if (!string.IsNullOrEmpty(policy))
        {
            tags.Add("policy", policy);
        }

        _authErrorsCounter.Add(1, tags);
    }
}
