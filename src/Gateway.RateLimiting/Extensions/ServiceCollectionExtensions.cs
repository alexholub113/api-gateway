using Gateway.RateLimiting.Configuration;
using Gateway.RateLimiting.Services;

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

        return services;
    }
}