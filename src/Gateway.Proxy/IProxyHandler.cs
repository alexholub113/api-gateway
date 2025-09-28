namespace Gateway.Proxy;

public interface IProxyHandler
{
    Task<Result> ProxyRequestAsync(HttpContext context, RouteMatch routeMatch, Uri uri);
}
