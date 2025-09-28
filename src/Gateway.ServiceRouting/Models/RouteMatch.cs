namespace Gateway.ServiceRouting.Models;

/// <summary>
/// Represents a successful route match for an incoming request
/// </summary>
public record RouteMatch(
    string ServiceId,
    string DownstreamPath,
    string? AuthPolicy = null,
    string? RateLimitPolicy = null,
    string? CachePolicy = null,
    string LoadBalancingStrategy = "RoundRobin",
    TimeSpan? Timeout = null
);