using Gateway.LoadBalancing.Abstractions;
using Gateway.LoadBalancing.Configuration;
using Gateway.LoadBalancing.Services;
using Microsoft.Extensions.DependencyInjection;

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
        services.AddSingleton<IHealthChecker, HealthCheckerService>();
        services.AddHostedService<HealthCheckerService>(provider => 
            (HealthCheckerService)provider.GetRequiredService<IHealthChecker>());
        
        return services;
    }
}