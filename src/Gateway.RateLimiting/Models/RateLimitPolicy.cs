namespace Gateway.RateLimiting.Models;

internal record RateLimitPolicy(
    int RequestsPerWindow,
    TimeSpan WindowSize,
    RateLimitAlgorithm Algorithm = RateLimitAlgorithm.SlidingWindow)
{
    public static RateLimitPolicy Create(int requestsPerSecond)
        => new(requestsPerSecond, TimeSpan.FromSeconds(1));

    public static RateLimitPolicy Create(int requests, TimeSpan window, RateLimitAlgorithm algorithm = RateLimitAlgorithm.SlidingWindow)
        => new(requests, window, algorithm);
}
