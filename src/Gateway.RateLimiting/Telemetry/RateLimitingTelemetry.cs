using System.Diagnostics.Metrics;

namespace Gateway.RateLimiting.Telemetry;

/// <summary>
/// Telemetry for rate limiting operations
/// </summary>
public sealed class RateLimitingTelemetry : IDisposable
{
    private readonly Meter _meter;

    // Counters
    private readonly Counter<long> _requestsTotal;
    private readonly Counter<long> _rateLimitRequestsTotal;
    private readonly Counter<long> _rateLimitPolicyUsage;
    private readonly Counter<long> _errorsTotal;

    // Histograms
    private readonly Histogram<double> _requestDuration;

    public RateLimitingTelemetry()
    {
        _meter = new Meter("Gateway.RateLimiting", "1.0.0");

        // Initialize counters
        _requestsTotal = _meter.CreateCounter<long>(
            "gateway_requests_total",
            "requests",
            "Total number of requests processed by the gateway");

        _rateLimitRequestsTotal = _meter.CreateCounter<long>(
            "gateway_rate_limit_requests_total",
            "requests",
            "Total rate limit checks with allowed/denied status");

        _rateLimitPolicyUsage = _meter.CreateCounter<long>(
            "gateway_rate_limit_policy_usage",
            "requests",
            "Usage count per rate limiting policy");

        _errorsTotal = _meter.CreateCounter<long>(
            "gateway_errors_total",
            "errors",
            "Total number of errors by type");

        // Initialize histograms
        _requestDuration = _meter.CreateHistogram<double>(
            "gateway_request_duration",
            "milliseconds",
            "Request processing duration");
    }

    /// <summary>
    /// Records a request with its basic metrics
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
    /// Records rate limit decision
    /// </summary>
    public void RecordRateLimitDecision(string serviceId, string policyName, bool allowed, string? clientId = null)
    {
        var status = allowed ? "allowed" : "denied";

        var rateLimitTags = new KeyValuePair<string, object?>[]
        {
            new("service_id", serviceId),
            new("policy_name", policyName),
            new("status", status),
            new("client_id", clientId ?? "unknown")
        };

        _rateLimitRequestsTotal.Add(1, rateLimitTags);

        // Record policy usage
        var policyTags = new KeyValuePair<string, object?>[]
        {
            new("service_id", serviceId),
            new("policy_name", policyName)
        };

        _rateLimitPolicyUsage.Add(1, policyTags);
    }

    /// <summary>
    /// Records rate limit violation
    /// </summary>
    public void RecordRateLimitViolation(string serviceId, string policyName, string? clientId = null)
    {
        RecordRateLimitDecision(serviceId, policyName, allowed: false, clientId);
    }

    public void Dispose()
    {
        _meter.Dispose();
    }
}