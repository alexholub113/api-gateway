namespace Gateway.Metrics.Models;

/// <summary>
/// Snapshot of current gateway metrics
/// </summary>
public record GatewayMetricsSnapshot
{
    public int RequestsPerMinute { get; init; }
    public int AverageResponseTimeMs { get; init; }
    public double CacheHitRatePercentage { get; init; }
    public int RateLimitedRequests { get; init; }
    public int AuthRequests { get; init; }
    public int ActiveUsers { get; init; }
    public int CircuitBreakersOpen { get; init; }
    public double UptimePercentage { get; init; }
    public int CacheSizeMB { get; init; }
    public int ActiveServices { get; init; }

    // Performance scores (0-100)
    public int OverallPerformanceScore { get; init; }
    public int LoadBalancingEfficiency { get; init; }
    public double ErrorRatePercentage { get; init; }

    // Trend indicators
    public TrendIndicator RequestsTrend { get; init; } = new();
    public TrendIndicator ResponseTimeTrend { get; init; } = new();
    public TrendIndicator CacheHitTrend { get; init; } = new();

    public DateTime Timestamp { get; init; }
}
