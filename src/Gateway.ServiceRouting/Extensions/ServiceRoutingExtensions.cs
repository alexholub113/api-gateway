using Gateway.ServiceRouting.Configuration;
using Gateway.ServiceRouting.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gateway.ServiceRouting.Extensions;

/// <summary>
/// Extension methods for registering routing services
/// </summary>
public static class ServiceRoutingExtensions
{
    /// <summary>
    /// Adds gateway routing services to the service collection
    /// </summary>
    public static IServiceCollection AddGatewayServiceRouting(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure routing options
        services.Configure<ServiceRoutingOptions>(
            configuration.GetSection(ServiceRoutingOptions.SectionName));

        services.AddSingleton<IRouteResolver, ServiceRouteResolver>();
        return services;
    }
}