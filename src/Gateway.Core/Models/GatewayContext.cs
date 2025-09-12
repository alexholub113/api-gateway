using Gateway.Core.Abstractions;

namespace Gateway.Core.Models;

/// <summary>
/// Default implementation of IGatewayContext
/// </summary>
internal class GatewayContext : IGatewayContext
{
    public RouteMatch? RouteMatch { get; set; }
    public ServiceInstance? SelectedInstance { get; set; }
    public AuthenticationResult? AuthResult { get; set; }
    public bool IsCircuitOpen { get; set; }
    public RequestMetrics Metrics { get; set; } = new();
    public IDictionary<string, object> Properties { get; } = new Dictionary<string, object>();
}