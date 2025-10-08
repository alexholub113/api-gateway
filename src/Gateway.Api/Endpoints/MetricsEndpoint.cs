using Gateway.Metrics;
using Gateway.Metrics.Models;
using MinimalEndpoints.Extensions;

namespace Gateway.Api.Endpoints;

public class MetricsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/api/metrics", GetMetrics)
            .WithName("GetMetrics")
            .WithTags("Metrics")
            .Produces<GatewayMetricsResponse>();
    }

    private static IResult GetMetrics(IGatewayMetricsProvider metricsProvider)
    {
        // Get real metrics from OpenTelemetry via the aggregator service
        var snapshot = metricsProvider.GetCurrentMetrics();

        var metrics = new GatewayMetricsResponse
        {
            RequestsPerMinute = snapshot.RequestsPerMinute,
            AverageResponseTimeMs = snapshot.AverageResponseTimeMs,
            CacheHitRatePercentage = snapshot.CacheHitRatePercentage,
            RateLimitedRequests = snapshot.RateLimitedRequests,
            AuthRequests = snapshot.AuthRequests,
            ActiveUsers = snapshot.ActiveUsers,
            CircuitBreakersOpen = snapshot.CircuitBreakersOpen,
            UptimePercentage = snapshot.UptimePercentage,
            CacheSizeMB = snapshot.CacheSizeMB,
            ActiveServices = snapshot.ActiveServices,
            OverallPerformanceScore = snapshot.OverallPerformanceScore,
            LoadBalancingEfficiency = snapshot.LoadBalancingEfficiency,
            ErrorRatePercentage = snapshot.ErrorRatePercentage,
            RequestsTrend = new TrendIndicator
            {
                PercentageChange = snapshot.RequestsTrend.PercentageChange,
                Direction = snapshot.RequestsTrend.Direction
            },
            ResponseTimeTrend = new TrendIndicator
            {
                PercentageChange = snapshot.ResponseTimeTrend.PercentageChange,
                Direction = snapshot.ResponseTimeTrend.Direction
            },
            CacheHitTrend = new TrendIndicator
            {
                PercentageChange = snapshot.CacheHitTrend.PercentageChange,
                Direction = snapshot.CacheHitTrend.Direction
            },
            Timestamp = snapshot.Timestamp
        };

        return Results.Ok(metrics);
    }
}

public record GatewayMetricsResponse
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

public record TrendIndicator
{
    public double PercentageChange { get; init; }
    public string Direction { get; init; } = "stable"; // "up", "down", "stable"
}