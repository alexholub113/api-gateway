using System.ComponentModel.DataAnnotations;

namespace Gateway.LoadBalancing.Configuration;

/// <summary>
/// Represents a specific instance of a service
/// </summary>
internal record ServiceInstance
{
    /// <summary>
    /// The URL of the service instance
    /// </summary>
    [Required(ErrorMessage = "URL is required")]
    [Url(ErrorMessage = "URL must be a valid HTTP or HTTPS URL")]
    public required string Url { get; init; }

    /// <summary>
    /// The weight for load balancing (must be greater than 0)
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Weight must be greater than 0")]
    public int Weight { get; init; } = 1;
}