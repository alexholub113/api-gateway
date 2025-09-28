namespace Gateway.LoadBalancing.Services;

/// <summary>
/// Service responsible for monitoring health of service instances
/// </summary>
internal interface IHealthChecker
{
    /// <summary>
    /// Gets the current health status of a service instance
    /// </summary>
    bool IsHealthy(string serviceName, string instanceUrl);
}
