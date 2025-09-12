namespace Gateway.Core.Abstractions;

/// <summary>
/// Represents a successful route match for an incoming request
/// </summary>
public record RouteMatch(
    string RouteId,
    string Pattern,
    string TargetServiceName,
    string? AuthPolicy = null,
    string? RateLimitPolicy = null,
    string? CachePolicy = null,
    string LoadBalancingStrategy = "RoundRobin",
    TimeSpan? Timeout = null
);