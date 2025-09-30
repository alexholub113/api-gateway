using Gateway.ServiceRouting.Services;

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
        services.AddSingleton<IRouteResolver, ServiceRouteResolver>();
        return services;
    }
}