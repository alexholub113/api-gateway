using Gateway.Routing.Abstractions;
using Gateway.Routing.Configuration;
using Gateway.Routing.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gateway.Routing.Extensions;

/// <summary>
/// Extension methods for registering routing services
/// </summary>
public static class RoutingExtensions
{
    /// <summary>
    /// Adds gateway routing services to the service collection
    /// </summary>
    public static IServiceCollection AddGatewayRouting(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure routing options
        services.Configure<RoutingOptions>(
            configuration.GetSection(RoutingOptions.SectionName));

        services.AddSingleton<IRouteResolver, RouteResolver>();
        return services;
    }

    /// <summary>
    /// Adds gateway routing middleware to the application pipeline
    /// </summary>
    public static IApplicationBuilder UseGatewayRouting(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RoutingMiddleware>();
    }
}