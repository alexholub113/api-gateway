using Gateway.Common;

namespace Gateway.LoadBalancing;

/// <summary>
/// Service responsible for selecting healthy service instances using load balancing strategies
/// </summary>
public interface ILoadBalancer
{
    /// <summary>
    /// Selects a healthy service instance for the given service name using the specified strategy
    /// </summary>
    /// <param name="serviceName">Name of the target service</param>
    /// <returns>Selected service instance or failure if no healthy instances available</returns>
    Result<Uri> SelectInstance(string serviceName);
}