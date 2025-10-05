using Gateway.LoadBalancing.Configuration;
using Gateway.LoadBalancing.Services;
using Gateway.LoadBalancing.Telemetry;

namespace Gateway.LoadBalancing.Extensions;

/// <summary>
/// Extension methods for registering load balancing services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds load balancing services to the service collection
    /// </summary>
    public static IServiceCollection AddLoadBalancing(this IServiceCollection services)
    {
        // Register configuration
        services.AddOptions<LoadBalancingOptions>()
            .BindConfiguration(LoadBalancingOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // Register services
        services.AddSingleton<ILoadBalancer, LoadBalancerService>();

        // Register HealthCheckerService as singleton for both interfaces
        services.AddSingleton<HealthCheckerService>();
        services.AddSingleton<IHealthChecker>(provider => provider.GetRequiredService<HealthCheckerService>());
        services.AddSingleton<IServiceStatusProvider>(provider => provider.GetRequiredService<HealthCheckerService>());

        // Register as hosted service
        services.AddHostedService(provider => provider.GetRequiredService<HealthCheckerService>());

        services.AddSingleton<LoadBalancingTelemetry>();

        return services;
    }
}