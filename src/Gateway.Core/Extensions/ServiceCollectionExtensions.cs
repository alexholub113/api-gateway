using Gateway.Core.Configuration;
using Gateway.Core.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Gateway.Core.Extensions;

/// <summary>
/// Extension methods for registering Gateway Core services
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Adds core gateway services to the service collection
    /// </summary>
    public static IServiceCollection AddGatewayCore(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure services options
        services.Configure<ServicesOptions>(
            configuration.GetSection(ServicesOptions.SectionName));

        return services;
    }

    /// <summary>
    /// Adds gateway core middleware to the application pipeline
    /// </summary>
    public static IApplicationBuilder UseGatewayCore(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GatewayContextMiddleware>();
    }
}