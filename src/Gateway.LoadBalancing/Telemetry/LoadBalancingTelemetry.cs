using System.Diagnostics.Metrics;

namespace Gateway.LoadBalancing.Telemetry;

/// <summary>
/// Telemetry for load balancing operations
/// </summary>
public sealed class LoadBalancingTelemetry : IDisposable
{
    private readonly Meter _meter;

    // Counters
    private readonly Counter<long> _loadBalancerRequestsTotal;
    private readonly Counter<long> _instanceFailuresTotal;

    // Histograms
    private readonly Histogram<double> _loadBalancerDuration;
    private readonly Histogram<double> _instanceSelectionDuration;

    // Gauges
    private readonly UpDownCounter<long> _instancesAvailable;
    private readonly UpDownCounter<long> _instancesTotal;
    private readonly UpDownCounter<long> _instancesHealthy;

    public LoadBalancingTelemetry()
    {
        _meter = new Meter("Gateway.LoadBalancing", "1.0.0");

        _loadBalancerRequestsTotal = _meter.CreateCounter<long>(
            "gateway_load_balancer_requests_total",
            "requests",
            "Requests by strategy and instance");

        _instanceFailuresTotal = _meter.CreateCounter<long>(
            "gateway_instance_failures_total",
            "failures",
            "Instance failure count");

        _loadBalancerDuration = _meter.CreateHistogram<double>(
            "gateway_load_balancer_duration",
            "milliseconds",
            "Time to select instance");

        _instanceSelectionDuration = _meter.CreateHistogram<double>(
            "gateway_instance_selection_duration",
            "milliseconds",
            "Time to select instance");

        _instancesAvailable = _meter.CreateUpDownCounter<long>(
            "gateway_instances_available",
            "instances",
            "Available instances per service");

        _instancesTotal = _meter.CreateUpDownCounter<long>(
            "gateway_instances_total",
            "instances",
            "Total configured instances per service");

        _instancesHealthy = _meter.CreateUpDownCounter<long>(
            "gateway_instances_healthy",
            "instances",
            "Healthy instances per service");
    }

    /// <summary>
    /// Records load balancer request
    /// </summary>
    public void RecordLoadBalancerRequest(string serviceId, string strategy, string instanceId, double durationMs)
    {
        var tags = new KeyValuePair<string, object?>[]
        {
            new("service_id", serviceId),
            new("strategy", strategy),
            new("instance_id", instanceId)
        };

        _loadBalancerRequestsTotal.Add(1, tags);
        _loadBalancerDuration.Record(durationMs, tags);
        _instanceSelectionDuration.Record(durationMs, tags);
    }

    /// <summary>
    /// Records instance failure
    /// </summary>
    public void RecordInstanceFailure(string serviceId, string instanceId, string failureType)
    {
        var tags = new KeyValuePair<string, object?>[]
        {
            new("service_id", serviceId),
            new("instance_id", instanceId),
            new("failure_type", failureType)
        };

        _instanceFailuresTotal.Add(1, tags);
    }

    /// <summary>
    /// Updates instance availability count
    /// </summary>
    public void UpdateInstancesAvailable(string serviceId, int delta)
    {
        var tags = new KeyValuePair<string, object?>[]
        {
            new("service_id", serviceId)
        };

        _instancesAvailable.Add(delta, tags);
    }

    /// <summary>
    /// Updates total instance count
    /// </summary>
    public void UpdateInstancesTotal(string serviceId, int delta)
    {
        var tags = new KeyValuePair<string, object?>[]
        {
            new("service_id", serviceId)
        };

        _instancesTotal.Add(delta, tags);
    }

    /// <summary>
    /// Updates healthy instance count
    /// </summary>
    public void UpdateInstancesHealthy(string serviceId, int delta)
    {
        var tags = new KeyValuePair<string, object?>[]
        {
            new("service_id", serviceId)
        };

        _instancesHealthy.Add(delta, tags);
    }

    public void Dispose()
    {
        _meter.Dispose();
    }
}