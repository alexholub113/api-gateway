using Gateway.Core.Services;
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
        services.AddSingleton<IGatewayHandler, GatewayHandler>();

        return services;
    }
}