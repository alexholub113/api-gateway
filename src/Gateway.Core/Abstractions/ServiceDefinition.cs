namespace Gateway.Core.Abstractions;

/// <summary>
/// Defines a backend service that can handle requests
/// </summary>
public record ServiceDefinition(
    string Name,
    IReadOnlyList<ServiceInstance> Instances,
    HealthCheckConfiguration? HealthCheck = null
);