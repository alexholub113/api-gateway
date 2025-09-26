using Gateway.Core.Abstractions;
using Gateway.Core.Configuration;
using Gateway.LoadBalancing.Abstractions;
using Gateway.LoadBalancing.Configuration;
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

    public Result<ServiceInstance> SelectInstance(string serviceName, string strategy = "RoundRobin")
    {
        var currentServicesOptions = servicesOptions.CurrentValue;
        var service = currentServicesOptions.Services.FirstOrDefault(s => s.Name == serviceName);

        if (service == null)
            return Result<ServiceInstance>.Failure($"Service '{serviceName}' not found");

        // Get only healthy instances
        var healthyInstances = service.Instances
            .Where(instance => healthChecker.IsHealthy(serviceName, instance.Url))
            .ToList();

        if (healthyInstances.Count == 0)
        {
            // Fallback to all instances if no healthy ones (fail-open strategy)
            healthyInstances = service.Instances.ToList();

            if (healthyInstances.Count == 0)
                return Result<ServiceInstance>.Failure($"No instances available for service '{serviceName}'");
        }

        var selectedInstance = strategy.ToLowerInvariant() switch
        {
            "roundrobin" => SelectRoundRobin(serviceName, healthyInstances),
            "weightedroundrobin" => SelectWeightedRoundRobin(serviceName, healthyInstances),
            "random" => SelectRandom(healthyInstances),
            "leastconnections" => SelectLeastConnections(healthyInstances),
            _ => SelectRoundRobin(serviceName, healthyInstances)
        };

        return Result<ServiceInstance>.Success(selectedInstance);
    }

    public Task<ServiceInstance?> SelectInstanceAsync(string serviceName, string strategy = "RoundRobin")
    {
        var result = SelectInstance(serviceName, strategy);
        return Task.FromResult(result.IsSuccess ? result.Value : null);
    }

    private ServiceInstance SelectRoundRobin(string serviceName, List<ServiceInstance> instances)
    {
        var counter = _roundRobinCounters.AddOrUpdate(serviceName, 0, (key, value) => (value + 1) % instances.Count);
        return instances[counter];
    }

    private ServiceInstance SelectWeightedRoundRobin(string serviceName, List<ServiceInstance> instances)
    {
        // Create a weighted list based on instance weights
        var weightedList = new List<ServiceInstance>();
        foreach (var instance in instances)
        {
            for (int i = 0; i < instance.Weight; i++)
            {
                weightedList.Add(instance);
            }
        }

        if (weightedList.Count == 0)
            return instances[0];

        var counter = _roundRobinCounters.AddOrUpdate(serviceName, 0, (key, value) => (value + 1) % weightedList.Count);
        return weightedList[counter];
    }

    private ServiceInstance SelectRandom(List<ServiceInstance> instances)
    {
        var index = _random.Next(instances.Count);
        return instances[index];
    }

    private ServiceInstance SelectLeastConnections(List<ServiceInstance> instances)
    {
        // For now, just return the first instance
        // In a real implementation, you'd track active connections per instance
        return instances[0];
    }
}