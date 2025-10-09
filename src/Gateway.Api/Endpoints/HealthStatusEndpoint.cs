using Gateway.Common.Configuration;
using Gateway.LoadBalancing;
using Gateway.LoadBalancing.Models;
using Microsoft.Extensions.Options;

namespace Gateway.Api.Endpoints;

public class HealthStatusEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/health-status", GetHealthStatus)
           .WithName("GetHealthStatus")
           .WithSummary("Gets health status of all configured services")
           .WithDescription("Returns health information for all services and their instances")
           .Produces<ServiceHealthStatus[]>();
    }

    private static IResult GetHealthStatus(
        IServiceStatusProvider serviceStatusProvider,
        IOptionsMonitor<GatewayOptions> gatewayOptions,
        ILogger<HealthStatusEndpoint> logger)
    {
        try
        {
            var instanceStatuses = serviceStatusProvider.GetAllInstanceStatuses();
            var services = gatewayOptions.CurrentValue.TargetServices;
            var healthStatuses = new List<ServiceHealthStatus>();

            foreach (var service in services)
            {
                var instances = new List<InstanceHealthStatus>();
                var healthyCount = 0;

                foreach (var instance in service.Instances)
                {
                    var key = new ServiceInstanceId(service.ServiceId, instance.Address);
                    var status = instanceStatuses.TryGetValue(key, out var instanceStatus) ? instanceStatus : null;

                    var isHealthy = status?.IsHealthy ?? false;
                    if (isHealthy) healthyCount++;

                    instances.Add(new InstanceHealthStatus(
                        Address: instance.Address,
                        Weight: instance.Weight,
                        IsHealthy: isHealthy,
                        LastChecked: status?.LastCheckTime ?? DateTime.UtcNow,
                        ConsecutiveFailures: status?.ConsecutiveFailures ?? 0
                    ));
                }

                healthStatuses.Add(new ServiceHealthStatus(
                    ServiceId: service.ServiceId,
                    LoadBalancingStrategy: service.LoadBalancingStrategy.ToString(),
                    TotalInstances: service.Instances.Length,
                    HealthyInstances: healthyCount,
                    RateLimitPolicy: service.RateLimitPolicy != null
                        ? $"{service.RateLimitPolicy.RequestsPerWindow} req/{service.RateLimitPolicy.WindowSize.TotalSeconds}s ({service.RateLimitPolicy.Algorithm})"
                        : null,
                    CachePolicy: service.CachePolicy != null
                        ? $"{service.CachePolicy.Duration.TotalMinutes}min cache"
                        : null,
                    Instances: instances.ToArray()
                ));
            }

            logger.LogDebug("Health status retrieved for {ServiceCount} services", healthStatuses.Count);
            return Results.Ok(healthStatuses.ToArray());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving health status");
            return Results.Problem("Failed to retrieve health status");
        }
    }
}

public record ServiceHealthStatus(
    string ServiceId,
    string LoadBalancingStrategy,
    int TotalInstances,
    int HealthyInstances,
    string? RateLimitPolicy,
    string? CachePolicy,
    InstanceHealthStatus[] Instances
);

public record InstanceHealthStatus(
    string Address,
    int Weight,
    bool IsHealthy,
    DateTime LastChecked,
    int ConsecutiveFailures
);