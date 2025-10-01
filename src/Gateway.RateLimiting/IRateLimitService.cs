namespace Gateway.RateLimiting;

public interface IRateLimitService
{
    Result ApplyRateLimit(HttpContext context, string policyName);
}