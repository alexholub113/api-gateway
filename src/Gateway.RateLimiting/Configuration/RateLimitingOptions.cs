using Gateway.RateLimiting.Models;

namespace Gateway.RateLimiting.Configuration;

internal class RateLimitingOptions
{
    public const string SectionName = "Gateway:RateLimiting";

    public Dictionary<string, RateLimitPolicy> Policies { get; set; } = [];
    public bool EnableGlobalRateLimit { get; set; } = false;
    public RateLimitPolicy? GlobalPolicy { get; set; }
}