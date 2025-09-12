namespace Gateway.Core.Abstractions;

/// <summary>
/// Health check configuration for a service
/// </summary>
public record HealthCheckConfiguration(
    string Path = "/health",
    TimeSpan Interval = default,
    TimeSpan Timeout = default
)
{
    public HealthCheckConfiguration() : this("/health", TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(5)) { }
}