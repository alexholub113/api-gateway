using Gateway.Auth.Services;
using Gateway.Auth.Telemetry;

namespace Gateway.Auth.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddGatewayAuth(this IServiceCollection services)
    {
        // Register services
        services.AddSingleton<JwtValidationService>();
        services.AddSingleton<AuthTelemetry>();

        return services;
    }
}
