namespace Gateway.LoadBalancing.Configuration;

/// <summary>
/// Defines a backend service that can handle requests
/// </summary>
internal record ServiceDefinition(
    string Name,
    IReadOnlyList<ServiceInstance> Instances,
    HealthCheckSettings? HealthCheck = null
);