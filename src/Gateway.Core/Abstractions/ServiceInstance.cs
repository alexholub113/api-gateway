namespace Gateway.Core.Abstractions;

/// <summary>
/// Represents a specific instance of a service
/// </summary>
public record ServiceInstance(
    string Url,
    int Weight = 1
);