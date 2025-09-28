using Gateway.Common;
using Gateway.LoadBalancing.Configuration;
using Gateway.LoadBalancing.Models;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace Gateway.LoadBalancing.Services;

/// <summary>
/// Load balancer implementation with support for multiple strategies
/// </summary>
internal class LoadBalancerService(
    IOptionsMonitor<ServicesOptions> servicesOptions,
    IOptionsMonitor<LoadBalancingOptions> loadBalancingOptions,
    IHealthChecker healthChecker) : ILoadBalancer
{
    private readonly ConcurrentDictionary<string, int> _roundRobinCounters = new();
    private readonly Random _random = new();

    public Result<Uri> SelectInstance(string serviceName)
    {
        var currentServicesOptions = servicesOptions.CurrentValue;
        var service = currentServicesOptions.Services.FirstOrDefault(s => s.Name == serviceName);

        if (service == null)
            return Result<Uri>.Failure($"Service '{serviceName}' not found");

        // Get only healthy instances
        var healthyInstances = service.Instances
            .Where(instance => healthChecker.IsHealthy(serviceName, instance.Url))
            .ToArray();
        if (healthyInstances.Length == 0)
        {
            return Result<Uri>.Failure($"No instances available for service '{serviceName}'");
        }

        var selectedInstance = loadBalancingOptions.CurrentValue.DefaultStrategy switch
        {
            LoadBalancingStrategy.RoundRobin => SelectRoundRobin(serviceName, healthyInstances),
            _ => SelectRoundRobin(serviceName, healthyInstances)
        };

        return Result<Uri>.Success(new Uri(selectedInstance.Url));
    }

    private ServiceInstance SelectRoundRobin(string serviceName, ServiceInstance[] instances)
    {
        var counter = _roundRobinCounters.AddOrUpdate(serviceName, 0, (key, value) => (value + 1) % instances.Length);
        return instances[counter];
    }
}