using Gateway.Common.Configuration;
using Gateway.RateLimiting.Models;

namespace Gateway.RateLimiting;

public interface IRateLimitService
{
    Result<RateLimitResult> ApplyRateLimit(HttpContext context, RateLimitPolicy policy);
}