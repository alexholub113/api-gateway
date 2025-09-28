namespace Gateway.Proxy.Configuration;

/// <summary>
/// Configuration options for the proxy module
/// </summary>
internal class ProxyOptions
{
    public const string SectionName = "Proxy";

    /// <summary>
    /// Timeout for proxy requests in seconds. Default is 30 seconds.
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Maximum number of retries for failed requests. Default is 3.
    /// </summary>
    public int MaxRetries { get; set; } = 3;

    /// <summary>
    /// Delay between retries in milliseconds. Default is 1000ms.
    /// </summary>
    public int RetryDelayMs { get; set; } = 1000;

    /// <summary>
    /// Whether to use exponential backoff for retries. Default is true.
    /// </summary>
    public bool UseExponentialBackoff { get; set; } = true;

    /// <summary>
    /// Circuit breaker failure threshold. Default is 5 consecutive failures.
    /// </summary>
    public int CircuitBreakerFailureThreshold { get; set; } = 5;

    /// <summary>
    /// Circuit breaker sampling duration in seconds. Default is 30 seconds.
    /// </summary>
    public int CircuitBreakerSamplingDurationSeconds { get; set; } = 30;

    /// <summary>
    /// Circuit breaker break duration in seconds. Default is 60 seconds.
    /// </summary>
    public int CircuitBreakerBreakDurationSeconds { get; set; } = 60;

    /// <summary>
    /// Whether to preserve the Host header from the original request
    /// </summary>
    public bool PreserveHostHeader { get; set; } = false;

    /// <summary>
    /// Headers to exclude when forwarding the request
    /// </summary>
    public string[] ExcludedHeaders { get; set; } =
    [
        "Host",
        "Connection",
        "Transfer-Encoding",
        "Upgrade",
        "Proxy-Connection",
        "Proxy-Authorization"
    ];
}