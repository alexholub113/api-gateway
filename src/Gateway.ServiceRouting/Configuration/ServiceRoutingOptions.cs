namespace Gateway.ServiceRouting.Configuration;

/// <summary>
/// Configuration options for routing
/// </summary>
internal class ServiceRoutingOptions
{
    public const string SectionName = "ServiceRouting";

    /// <summary>
    /// URL prefix/postfix to identify routing requests (e.g., "route")
    /// Gateway URLs will be: /{RoutePrefix}/{serviceId}/**
    /// </summary>
    public string RoutePrefix { get; set; } = "route";

    /// <summary>
    /// List of services that can be routed to
    /// </summary>
    public List<ServiceRouteConfiguration> Routes { get; set; } = new();
}

/// <summary>
/// Configuration for a service route
/// </summary>
internal record ServiceRouteConfiguration(
    string ServiceId,
    string TargetService,
    string[] Methods,
    string? AuthPolicy = null,
    string? RateLimitPolicy = null,
    string? CachePolicy = null,
    string LoadBalancingStrategy = "RoundRobin",
    TimeSpan? Timeout = null
);