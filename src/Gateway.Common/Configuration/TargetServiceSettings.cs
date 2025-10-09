using Gateway.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace Gateway.Common.Configuration;

public class TargetServiceSettings
{
    [Required]
    public string ServiceId { get; set; } = null!;

    public LoadBalancingStrategy LoadBalancingStrategy { get; set; } = LoadBalancingStrategy.RoundRobin;

    [Required]
    public ServiceInstance[] Instances { get; set; } = [];

    public RateLimitPolicy? RateLimitPolicy { get; set; }
    public CachePolicy? CachePolicy { get; set; }
    public AuthPolicy? AuthPolicy { get; set; }
}
