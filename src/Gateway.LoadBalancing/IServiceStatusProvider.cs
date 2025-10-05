using Gateway.LoadBalancing.Models;

namespace Gateway.LoadBalancing;
public interface IServiceStatusProvider
{
    IDictionary<ServiceInstanceId, InstanceHealthStatus> GetAllInstanceStatuses();
}
