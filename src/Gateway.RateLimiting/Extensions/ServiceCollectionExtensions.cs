using Gateway.RateLimiting.Configuration;
using Gateway.RateLimiting.Services;
using Gateway.RateLimiting.Telemetry;

namespace Gateway.RateLimiting.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRateLimiting(this IServiceCollection services)
    {
        services.AddOptions<RateLimitingOptions>()
            .BindConfiguration(RateLimitingOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddSingleton<IRateLimitService, RateLimitService>();
        services.AddSingleton<RateLimitingTelemetry>();

        return services;
    }
}