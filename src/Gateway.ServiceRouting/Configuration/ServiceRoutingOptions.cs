namespace Gateway.ServiceRouting.Configuration;

/// <summary>
/// Configuration options for routing
/// </summary>
internal class ServiceRoutingOptions
{
    public const string SectionName = "ServiceRouting";

    public List<ServiceRouteConfiguration> Routes { get; set; } = new();
}

/// <summary>
/// Configuration for a specific route
/// </summary>
internal record ServiceRouteConfiguration(
    string Id,
    string Path,
    string[] Methods,
    string TargetService,
    string? AuthPolicy = null,
    string? RateLimitPolicy = null,
    string? CachePolicy = null,
    string LoadBalancingStrategy = "RoundRobin",
    TimeSpan? Timeout = null
);