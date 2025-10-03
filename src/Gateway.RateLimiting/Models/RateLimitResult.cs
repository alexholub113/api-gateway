namespace Gateway.RateLimiting.Models;
public record RateLimitResult(bool IsAllowed, int RemainingRequests, TimeSpan RetryAfter);
