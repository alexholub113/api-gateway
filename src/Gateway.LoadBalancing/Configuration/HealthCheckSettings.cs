namespace Gateway.LoadBalancing.Configuration;

/// <summary>
/// Health check configuration for a service
/// </summary>
internal record HealthCheckSettings(
    string Path = "/health",
    TimeSpan Interval = default,
    TimeSpan Timeout = default
)
{
    public HealthCheckSettings() : this("/health", TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(5)) { }
}