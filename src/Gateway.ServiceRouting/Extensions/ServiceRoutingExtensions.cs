using Gateway.ServiceRouting.Abstractions;
using Gateway.ServiceRouting.Configuration;
using Gateway.ServiceRouting.Services;
using Microsoft.AspNetCore.Builder;
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

    /// <summary>
    /// Adds gateway routing middleware to the application pipeline
    /// </summary>
    public static IApplicationBuilder UseGatewayServiceRouting(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ServiceRoutingMiddleware>();
    }
}