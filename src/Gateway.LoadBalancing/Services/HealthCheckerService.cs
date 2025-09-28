using Gateway.LoadBalancing.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System.Collections.Concurrent;

namespace Gateway.LoadBalancing.Services;

/// <summary>
/// Background service that monitors health of service instances
/// </summary>
internal class HealthCheckerService(
    IOptionsMonitor<ServicesOptions> servicesOptions,
    IOptionsMonitor<LoadBalancingOptions> loadBalancingOptions,
    IHttpClientFactory httpClientFactory) : BackgroundService, IHealthChecker
{
    private readonly ConcurrentDictionary<string, InstanceHealthStatus> _healthStatuses = new();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        InitializeHealthStatuses();

        while (!stoppingToken.IsCancellationRequested)
        {
            await CheckAllInstancesHealth();

            var options = loadBalancingOptions.CurrentValue;
            await Task.Delay(TimeSpan.FromSeconds(options.HealthCheckIntervalSeconds), stoppingToken);
        }
    }

    public bool IsHealthy(string serviceName, string instanceUrl)
    {
        var key = $"{serviceName}:{instanceUrl}";
        return !_healthStatuses.TryGetValue(key, out var status) || status.IsHealthy;
    }

    private void InitializeHealthStatuses()
    {
        var services = servicesOptions.CurrentValue.Services;
        foreach (var service in services)
        {
            foreach (var instance in service.Instances)
            {
                var key = $"{service.Name}:{instance.Url}";
                _healthStatuses.TryAdd(key, new InstanceHealthStatus(true, 0, DateTime.UtcNow));
            }
        }
    }

    private async Task CheckAllInstancesHealth()
    {
        var services = servicesOptions.CurrentValue.Services;
        var options = loadBalancingOptions.CurrentValue;

        var healthCheckTasks = new List<Task>();

        foreach (var service in services)
        {
            foreach (var instance in service.Instances)
            {
                var task = CheckInstanceHealth(service.Name, instance.Url, service.HealthCheck, options);
                healthCheckTasks.Add(task);
            }
        }

        await Task.WhenAll(healthCheckTasks);
    }

    private async Task CheckInstanceHealth(string serviceName, string instanceUrl, HealthCheckSettings? healthCheck, LoadBalancingOptions options)
    {
        var key = $"{serviceName}:{instanceUrl}";

        try
        {
            if (healthCheck?.Path == null)
            {
                // No health check configured, assume healthy
                UpdateHealthStatus(key, true);
                return;
            }

            var httpClient = httpClientFactory.CreateClient();
            httpClient.Timeout = TimeSpan.FromSeconds(options.HealthCheckTimeoutSeconds);

            var healthUrl = $"{instanceUrl.TrimEnd('/')}{healthCheck.Path}";
            var response = await httpClient.GetAsync(healthUrl);

            var isHealthy = response.IsSuccessStatusCode;
            UpdateHealthStatus(key, isHealthy);
        }
        catch (Exception)
        {
            UpdateHealthStatus(key, false);
        }
    }

    private void UpdateHealthStatus(string key, bool isHealthy)
    {
        var options = loadBalancingOptions.CurrentValue;

        _healthStatuses.AddOrUpdate(key,
            new InstanceHealthStatus(isHealthy, isHealthy ? 0 : 1, DateTime.UtcNow),
            (k, existing) =>
            {
                var consecutiveFailures = isHealthy ? 0 : existing.ConsecutiveFailures + 1;
                var shouldBeHealthy = isHealthy || consecutiveFailures < options.MaxConsecutiveFailures;

                // If instance was unhealthy, check retry delay
                if (!existing.IsHealthy && !isHealthy)
                {
                    var timeSinceLastCheck = DateTime.UtcNow - existing.LastCheckTime;
                    if (timeSinceLastCheck < TimeSpan.FromSeconds(options.UnhealthyRetryDelaySeconds))
                    {
                        shouldBeHealthy = false;
                    }
                }

                return new InstanceHealthStatus(shouldBeHealthy, consecutiveFailures, DateTime.UtcNow);
            });
    }

    private record InstanceHealthStatus(bool IsHealthy, int ConsecutiveFailures, DateTime LastCheckTime);
}