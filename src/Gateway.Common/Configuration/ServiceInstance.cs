using System.ComponentModel.DataAnnotations;

namespace Gateway.Common.Configuration;

/// <summary>
/// Represents a specific instance of a service
/// </summary>
public record ServiceInstance
{
    /// <summary>
    /// The Address of the service instance
    /// </summary>
    [Required(ErrorMessage = "Address is required")]
    [Url(ErrorMessage = "Address must be a valid HTTP or HTTPS URL")]
    public required string Address { get; init; }

    /// <summary>
    /// The weight for load balancing (must be greater than 0)
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Weight must be greater than 0")]
    public int Weight { get; init; } = 1;
}