namespace Gateway.Common.Configuration;

public enum RateLimitAlgorithm
{
    SlidingWindow,
    TokenBucket,
    FixedWindow
}
