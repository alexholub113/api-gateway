using Gateway.Common.Models;
using System.ComponentModel.DataAnnotations;

namespace Gateway.Core.Configuration;

internal class TargetServiceSettings
{
    [Required]
    public string ServiceId { get; set; } = null!;

    public LoadBalancingStrategy LoadBalancingStrategy { get; set; } = LoadBalancingStrategy.RoundRobin;

    [Required]
    public ServiceInstance[] Instances { get; set; } = [];
}
