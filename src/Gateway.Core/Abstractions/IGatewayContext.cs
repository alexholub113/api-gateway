namespace Gateway.Core.Abstractions;

/// <summary>
/// Shared context that flows through the gateway pipeline for each request
/// </summary>
public interface IGatewayContext
{
    /// <summary>
    /// The matched route information for this request
    /// </summary>
    RouteMatch? RouteMatch { get; set; }
    
    /// <summary>
    /// The selected service instance for load balancing
    /// </summary>
    ServiceInstance? SelectedInstance { get; set; }
    
    /// <summary>
    /// Authentication result from AuthZN service
    /// </summary>
    AuthenticationResult? AuthResult { get; set; }
    
    /// <summary>
    /// Indicates if circuit breaker is open for the target service
    /// </summary>
    bool IsCircuitOpen { get; set; }
    
    /// <summary>
    /// Request metrics collected during processing
    /// </summary>
    RequestMetrics Metrics { get; set; }
    
    /// <summary>
    /// Additional context data that modules can use for inter-module communication
    /// </summary>
    IDictionary<string, object> Properties { get; }
}