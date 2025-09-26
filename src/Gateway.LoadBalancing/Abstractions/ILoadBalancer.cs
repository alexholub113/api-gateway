using Gateway.Core.Abstractions;

namespace Gateway.LoadBalancing.Abstractions;

/// <summary>
/// Service responsible for selecting healthy service instances using load balancing strategies
/// </summary>
internal interface ILoadBalancer
{
    /// <summary>
    /// Selects a healthy service instance for the given service name using the specified strategy
    /// </summary>
    /// <param name="serviceName">Name of the target service</param>
    /// <param name="strategy">Load balancing strategy to use</param>
    /// <returns>Selected service instance or failure if no healthy instances available</returns>
    Result<ServiceInstance> SelectInstance(string serviceName, string strategy = "RoundRobin");
    
    /// <summary>
    /// Async version of SelectInstance for better async/await patterns
    /// </summary>
    Task<ServiceInstance?> SelectInstanceAsync(string serviceName, string strategy = "RoundRobin");
}

/// <summary>
/// Service responsible for monitoring health of service instances
/// </summary>
internal interface IHealthChecker
{
    /// <summary>
    /// Starts background health checking for all registered services
    /// </summary>
    Task StartAsync(CancellationToken cancellationToken);
    
    /// <summary>
    /// Stops background health checking
    /// </summary>
    Task StopAsync(CancellationToken cancellationToken);
    
    /// <summary>
    /// Gets the current health status of a service instance
    /// </summary>
    bool IsHealthy(string serviceName, string instanceUrl);
}