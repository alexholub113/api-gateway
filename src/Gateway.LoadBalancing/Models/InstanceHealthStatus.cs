namespace Gateway.LoadBalancing.Models;
public record InstanceHealthStatus(bool IsHealthy, int ConsecutiveFailures, DateTime LastCheckTime);
