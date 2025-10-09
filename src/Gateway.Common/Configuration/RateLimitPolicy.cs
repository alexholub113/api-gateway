namespace Gateway.Common.Configuration;

public class RateLimitPolicy
{
    public int RequestsPerWindow { get; set; }
    public TimeSpan WindowSize { get; set; }
    public RateLimitAlgorithm Algorithm { get; set; } = RateLimitAlgorithm.SlidingWindow;
}
