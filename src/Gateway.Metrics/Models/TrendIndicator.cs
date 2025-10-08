namespace Gateway.Metrics.Models;

/// <summary>
/// Represents a trend indicator for a metric
/// </summary>
public record TrendIndicator
{
    public double PercentageChange { get; init; }
    public string Direction { get; init; } = "stable"; // "up", "down", "stable"
}
