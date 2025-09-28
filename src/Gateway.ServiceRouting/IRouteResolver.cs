namespace Gateway.ServiceRouting;

/// <summary>
/// Service responsible for resolving routes based on incoming requests
/// </summary>
public interface IRouteResolver
{
    /// <summary>
    /// Resolves a route for the given path and HTTP method
    /// </summary>
    Result<RouteMatch> ResolveRoute(string serviceId, string method, string downstreamPath);
}