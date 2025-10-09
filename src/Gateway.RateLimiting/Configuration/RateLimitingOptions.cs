namespace Gateway.RateLimiting.Configuration;

internal class RateLimitingOptions
{
    public const string SectionName = "Gateway:RateLimiting";

    public bool EnableGlobalRateLimit { get; set; } = false;
}