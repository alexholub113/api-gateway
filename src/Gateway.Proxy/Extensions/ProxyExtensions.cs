
using Gateway.Proxy.Configuration;
using Gateway.Proxy.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Polly;

namespace Gateway.Proxy.Extensions;

/// <summary>
/// Extension methods for registering and configuring the proxy module
/// </summary>
public static class ProxyExtensions
{
    /// <summary>
    /// Adds the proxy module services to the dependency injection container
    /// </summary>
    public static IServiceCollection AddGatewayProxy(this IServiceCollection services, IConfiguration configuration)
    {
        // Configure proxy options
        services.Configure<ProxyOptions>(configuration.GetSection("Gateway:Proxy"));

        // Register the proxy service
        services.AddSingleton<HttpProxyService>();

        // Configure HTTP client with resilience policies
        var proxyOptions = configuration.GetSection("Gateway:Proxy").Get<ProxyOptions>() ?? new ProxyOptions();

        services.AddHttpClient(ProxyConstants.HttpClientName)
            .ConfigureHttpClient(client =>
            {
                client.Timeout = TimeSpan.FromSeconds(proxyOptions.TimeoutSeconds);
            })
            .AddStandardResilienceHandler(options =>
            {
                // Configure retry policy
                options.Retry.MaxRetryAttempts = proxyOptions.MaxRetries;
                options.Retry.UseJitter = true;
                options.Retry.BackoffType = proxyOptions.UseExponentialBackoff
                    ? DelayBackoffType.Exponential
                    : DelayBackoffType.Constant;
                options.Retry.Delay = TimeSpan.FromMilliseconds(proxyOptions.RetryDelayMs);

                // Configure circuit breaker
                options.CircuitBreaker.FailureRatio = 0.5;
                options.CircuitBreaker.MinimumThroughput = proxyOptions.CircuitBreakerFailureThreshold;
                options.CircuitBreaker.SamplingDuration = TimeSpan.FromSeconds(proxyOptions.CircuitBreakerSamplingDurationSeconds);
                options.CircuitBreaker.BreakDuration = TimeSpan.FromSeconds(proxyOptions.CircuitBreakerBreakDurationSeconds);

                // Configure timeout
                options.TotalRequestTimeout.Timeout = TimeSpan.FromSeconds(proxyOptions.TimeoutSeconds);
            });

        return services;
    }

    /// <summary>
    /// Adds the proxy middleware to the application pipeline
    /// </summary>
    public static IApplicationBuilder UseGatewayProxy(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ProxyMiddleware>();
    }
}