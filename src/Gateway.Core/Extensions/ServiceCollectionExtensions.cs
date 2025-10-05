using Gateway.Caching.Extensions;
using Gateway.Core.Services;
using Gateway.LoadBalancing.Extensions;
using Gateway.Metrics.Extensions;
using Gateway.Proxy.Extensions;
using Gateway.RateLimiting.Extensions;

namespace Gateway.Core.Extensions;

/// <summary>
/// Extension methods for registering Gateway Core services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds core gateway services to the service collection
    /// </summary>
    public static IServiceCollection AddGateway(this IServiceCollection services, IConfiguration configuration)
    {
        // Add gateway services
        services.AddCommonServices()
            .AddGatewayTelemetry()
            .AddGatewayProxy(configuration)
            .AddLoadBalancing()
            .AddRateLimiting()
            .AddCaching();

        // Configure services options
        services.AddSingleton<IGatewayHandler, GatewayHandler>();

        return services;
    }
}