using Gateway.Caching.Configuration;
using Gateway.Caching.Services;
using Gateway.Caching.Telemetry;

namespace Gateway.Caching.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddCaching(this IServiceCollection services)
    {
        // Configure options
        services.AddOptions<CachingOptions>()
            .BindConfiguration(CachingOptions.SectionName)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        // Add memory cache
        services.AddMemoryCache(options =>
        {
            // Configure from options when building service provider
            var serviceProvider = services.BuildServiceProvider();
            var cachingOptions = serviceProvider.GetService<IOptions<CachingOptions>>()?.Value;

            if (cachingOptions?.Memory != null)
            {
                options.SizeLimit = cachingOptions.Memory.SizeLimit;
            }
        });

        // Register cache service
        services.AddSingleton<ICacheService, MemoryCacheService>();
        services.AddSingleton<CachingTelemetry>();

        return services;
    }
}