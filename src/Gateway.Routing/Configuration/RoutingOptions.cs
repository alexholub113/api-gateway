namespace Gateway.Routing.Configuration;

/// <summary>
/// Configuration options for routing
/// </summary>
internal class RoutingOptions
{
    public const string SectionName = "Routing";

    public List<RouteConfiguration> Routes { get; set; } = new();
}

/// <summary>
/// Configuration for a specific route
/// </summary>
internal record RouteConfiguration(
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