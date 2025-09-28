using Gateway.LoadBalancing.Models;

namespace Gateway.LoadBalancing.Configuration;

/// <summary>
/// Configuration options for the load balancing module
/// </summary>
internal class LoadBalancingOptions
{
    public const string SectionName = "LoadBalancing";

    /// <summary>
    /// Default load balancing strategy to use when not specified
    /// </summary>
    public LoadBalancingStrategy DefaultStrategy { get; set; }

    /// <summary>
    /// Health check interval in seconds
    /// </summary>
    public int HealthCheckIntervalSeconds { get; set; } = 30;

    /// <summary>
    /// Health check timeout in seconds
    /// </summary>
    public int HealthCheckTimeoutSeconds { get; set; } = 5;

    /// <summary>
    /// Maximum number of consecutive health check failures before marking instance as unhealthy
    /// </summary>
    public int MaxConsecutiveFailures { get; set; } = 3;

    /// <summary>
    /// Time to wait before retrying an unhealthy instance (in seconds)
    /// </summary>
    public int UnhealthyRetryDelaySeconds { get; set; } = 60;
}