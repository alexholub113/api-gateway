using Gateway.Common.Configuration;
using Gateway.LoadBalancing.Models;
using Microsoft.Extensions.Logging;

namespace Gateway.LoadBalancing.Services;

/// <summary>
/// Background service that monitors health of service instances
/// </summary>
internal class HealthCheckerService(
    IOptionsMonitor<GatewayOptions> servicesOptions,
    IOptionsMonitor<LoadBalancingOptions> loadBalancingOptions,
    IHttpClientFactory httpClientFactory,
    ILogger<HealthCheckerService> logger) : BackgroundService, IHealthChecker, IServiceStatusProvider
{
    private readonly ConcurrentDictionary<ServiceInstanceId, InstanceHealthStatus> _healthStatuses = new();

    public IDictionary<ServiceInstanceId, InstanceHealthStatus> GetAllInstanceStatuses()
    {
        return new Dictionary<ServiceInstanceId, InstanceHealthStatus>(_healthStatuses);
    }

    public bool IsHealthy(ServiceInstanceId serviceInstanceId)
    {
        return !_healthStatuses.TryGetValue(serviceInstanceId, out var status) || status.IsHealthy;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        InitializeHealthStatuses();

        while (!stoppingToken.IsCancellationRequested)
        {
            var startTime = DateTime.UtcNow;

            try
            {
                await CheckAllInstancesHealth();
                var duration = DateTime.UtcNow - startTime;
                logger.LogDebug("Health check cycle completed in {Duration}ms", duration.TotalMilliseconds);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during health check cycle");
            }

            var options = loadBalancingOptions.CurrentValue;
            await Task.Delay(TimeSpan.FromSeconds(options.HealthCheckIntervalSeconds), stoppingToken);
        }

        logger.LogInformation("Health checker service stopping");
    }

    private void InitializeHealthStatuses()
    {
        var services = servicesOptions.CurrentValue.TargetServices;
        var totalInstances = 0;

        foreach (var service in services)
        {
            foreach (var instance in service.Instances)
            {
                var key = new ServiceInstanceId(service.ServiceId, instance.Address);
                _healthStatuses.TryAdd(key, new InstanceHealthStatus(false, 0, DateTime.UtcNow));
                totalInstances++;
            }
        }

        logger.LogInformation("Initialized health status tracking for {ServiceCount} services with {InstanceCount} total instances", services.Length, totalInstances);
    }

    private async Task CheckAllInstancesHealth()
    {
        var services = servicesOptions.CurrentValue.TargetServices;
        var options = loadBalancingOptions.CurrentValue;

        var healthCheckTasks = new List<Task>();

        foreach (var service in services)
        {
            foreach (var instance in service.Instances)
            {
                var task = CheckInstanceHealth(service.ServiceId, instance.Address, options);
                healthCheckTasks.Add(task);
            }
        }

        await Task.WhenAll(healthCheckTasks);
    }

    private async Task CheckInstanceHealth(string serviceId, string instanceUrl, LoadBalancingOptions options)
    {
        var key = new ServiceInstanceId(serviceId, instanceUrl);
        var healthUrl = $"{instanceUrl.TrimEnd('/')}{options.HealthCheckPath}";

        try
        {
            var httpClient = httpClientFactory.CreateClient();
            httpClient.Timeout = TimeSpan.FromSeconds(options.HealthCheckTimeoutSeconds);

            logger.LogDebug("Checking health for instance '{InstanceUrl}' at '{HealthUrl}'", instanceUrl, healthUrl);

            var response = await httpClient.GetAsync(healthUrl);

            var isHealthy = response.IsSuccessStatusCode;
            logger.LogDebug("Health check for '{InstanceUrl}' returned {StatusCode} - {HealthStatus}", instanceUrl, response.StatusCode, isHealthy ? "Healthy" : "Unhealthy");

            UpdateHealthStatus(key, isHealthy);
        }
        catch (Exception ex)
        {
            logger.LogWarning("Health check failed for instance '{InstanceUrl}' at '{HealthUrl}': {Error}", instanceUrl, healthUrl, ex.Message);
            UpdateHealthStatus(key, false);
        }
    }

    private void UpdateHealthStatus(ServiceInstanceId key, bool isHealthy)
    {
        var options = loadBalancingOptions.CurrentValue;
        var previousStatus = _healthStatuses.TryGetValue(key, out var existing) ? existing : null;

        var newStatus = _healthStatuses.AddOrUpdate(key,
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

        // Log health status changes
        if (previousStatus == null || previousStatus.IsHealthy != newStatus.IsHealthy)
        {
            if (newStatus.IsHealthy)
            {
                logger.LogInformation("Instance '{Key}' is now HEALTHY", key);
            }
            else
            {
                logger.LogWarning("Instance '{Key}' is now UNHEALTHY (consecutive failures: {ConsecutiveFailures})", key, newStatus.ConsecutiveFailures);
            }
        }
    }
}