using Gateway.RateLimiting.Models;

namespace Gateway.RateLimiting;

public interface IRateLimitService
{
    Result<RateLimitResult> ApplyRateLimit(HttpContext context, string policyName);
}