using System.Diagnostics.Metrics;

namespace Gateway.Caching.Telemetry;

/// <summary>
/// Telemetry for caching operations
/// </summary>
public sealed class CachingTelemetry : IDisposable
{
    private readonly Meter _meter;

    // Counters
    private readonly Counter<long> _cacheRequestsTotal;
    private readonly Counter<long> _cacheHits;
    private readonly Counter<long> _cacheMisses;

    // Gauges
    private readonly UpDownCounter<long> _cacheSizeBytes;
    private readonly Histogram<double> _cacheHitRatio;

    public CachingTelemetry()
    {
        _meter = new Meter("Gateway.Caching", "1.0.0");

        _cacheRequestsTotal = _meter.CreateCounter<long>(
            "gateway_cache_requests_total",
            "requests",
            "Cache requests by hit/miss status");

        _cacheHits = _meter.CreateCounter<long>(
            "gateway_cache_hits_total",
            "hits",
            "Total cache hits");

        _cacheMisses = _meter.CreateCounter<long>(
            "gateway_cache_misses_total",
            "misses",
            "Total cache misses");

        _cacheSizeBytes = _meter.CreateUpDownCounter<long>(
            "gateway_cache_size_bytes",
            "bytes",
            "Total cache size in bytes");

        _cacheHitRatio = _meter.CreateHistogram<double>(
            "gateway_cache_hit_ratio",
            "ratio",
            "Cache hit percentage");
    }

    /// <summary>
    /// Records a cache hit
    /// </summary>
    public void RecordCacheHit(string serviceId, string policy, long sizeBytes = 0)
    {
        var tags = new KeyValuePair<string, object?>[]
        {
            new("service_id", serviceId),
            new("policy", policy),
            new("status", "hit")
        };

        _cacheRequestsTotal.Add(1, tags);
        _cacheHits.Add(1, tags);

        if (sizeBytes > 0)
        {
            _cacheSizeBytes.Add(sizeBytes, tags);
        }
    }

    /// <summary>
    /// Records a cache miss
    /// </summary>
    public void RecordCacheMiss(string serviceId, string policy)
    {
        var tags = new KeyValuePair<string, object?>[]
        {
            new("service_id", serviceId),
            new("policy", policy),
            new("status", "miss")
        };

        _cacheRequestsTotal.Add(1, tags);
        _cacheMisses.Add(1, tags);
    }

    /// <summary>
    /// Records cache size change
    /// </summary>
    public void RecordCacheSizeChange(long deltaBytes, string serviceId, string policy)
    {
        var tags = new KeyValuePair<string, object?>[]
        {
            new("service_id", serviceId),
            new("policy", policy)
        };

        _cacheSizeBytes.Add(deltaBytes, tags);
    }

    /// <summary>
    /// Records cache hit ratio
    /// </summary>
    public void RecordCacheHitRatio(double ratio, string serviceId, string policy)
    {
        var tags = new KeyValuePair<string, object?>[]
        {
            new("service_id", serviceId),
            new("policy", policy)
        };

        _cacheHitRatio.Record(ratio, tags);
    }

    public void Dispose()
    {
        _meter.Dispose();
    }
}