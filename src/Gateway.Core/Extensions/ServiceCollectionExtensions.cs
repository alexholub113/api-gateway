using Gateway.Common.Configuration;
using Gateway.Core.Services;
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
    public static IServiceCollection AddGatewayCore(this IServiceCollection services)
    {
        services.AddOptions<GatewayOptions>()
            .BindConfiguration(GatewayOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // Add rate limiting services
        services.AddRateLimiting();

        // Configure services options
        services.AddSingleton<IGatewayHandler, GatewayHandler>();

        return services;
    }
}