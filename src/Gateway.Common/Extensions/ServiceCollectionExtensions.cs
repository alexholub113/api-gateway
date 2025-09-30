using Gateway.Common.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gateway.Common.Extensions;

/// <summary>
/// Extension methods for registering common services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds common services to the service collection
    /// </summary>
    public static IServiceCollection AddCommonServices(this IServiceCollection services)
    {
        // Register configuration
        services.AddOptions<GatewayOptions>()
            .BindConfiguration(GatewayOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
}