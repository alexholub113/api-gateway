namespace Gateway.RateLimiting.Models;
internal enum RateLimitAlgorithm
{
    SlidingWindow,
    TokenBucket,
    FixedWindow
}
