using Gateway.LoadBalancing.Models;

namespace Gateway.LoadBalancing.Services;

/// <summary>
/// Service responsible for monitoring health of service instances
/// </summary>
public interface IHealthChecker
{
    /// <summary>
    /// Gets the current health status of a service instance
    /// </summary>
    bool IsHealthy(ServiceInstanceId serviceInstanceId);
}
