using Gateway.Common.Configuration;
using Gateway.Common.Models;
using Gateway.LoadBalancing.Models;
using Microsoft.Extensions.Logging;

namespace Gateway.LoadBalancing.Services;

/// <summary>
/// Load balancer implementation with support for multiple strategies
/// </summary>
internal class LoadBalancerService(
    IOptionsMonitor<GatewayOptions> servicesOptions,
    IOptionsMonitor<LoadBalancingOptions> loadBalancingOptions,
    IHealthChecker healthChecker,
    ILogger<LoadBalancerService> logger) : ILoadBalancer
{
    private readonly ConcurrentDictionary<string, int> _roundRobinCounters = new();
    private readonly Random _random = new();

    public Result<Uri> SelectInstance(string serviceId)
    {
        var currentServicesOptions = servicesOptions.CurrentValue;
        var service = currentServicesOptions.TargetServices.FirstOrDefault(s => s.ServiceId == serviceId);

        if (service == null)
        {
            logger.LogWarning("Service '{TargetServiceId}' not found in configuration", serviceId);
            return Result<Uri>.Failure($"Service '{serviceId}' not found");
        }

        // Get only healthy instances
        var healthyInstances = service.Instances
            .Where(instance => healthChecker.IsHealthy(new ServiceInstanceId(serviceId, instance.Address)))
            .ToArray();
        if (healthyInstances.Length == 0)
        {
            logger.LogError("No healthy instances available for service '{TargetServiceId}'", serviceId);
            return Result<Uri>.Failure($"No instances available for service '{serviceId}'");
        }

        var strategy = loadBalancingOptions.CurrentValue.DefaultStrategy;

        var selectedInstance = strategy switch
        {
            LoadBalancingStrategy.RoundRobin => SelectRoundRobin(serviceId, healthyInstances),
            _ => SelectRoundRobin(serviceId, healthyInstances)
        };

        return Result<Uri>.Success(new Uri(selectedInstance.Address));
    }

    private ServiceInstance SelectRoundRobin(string serviceName, ServiceInstance[] instances)
    {
        var counter = _roundRobinCounters.AddOrUpdate(serviceName, 0, (key, value) => (value + 1) % instances.Length);
        return instances[counter];
    }
}