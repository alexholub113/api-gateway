namespace Gateway.Core.Abstractions;

/// <summary>
/// Metrics collected during request processing
/// </summary>
public record RequestMetrics(
    DateTime StartTime,
    TimeSpan? Duration = null,
    int? StatusCode = null,
    long? RequestSize = null,
    long? ResponseSize = null
)
{
    public RequestMetrics() : this(DateTime.UtcNow) { }
}