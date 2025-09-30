namespace Gateway.Proxy;

public interface IProxyHandler
{
    Task<Result> ProxyRequestAsync(HttpContext context, Uri uri, string downstreamPath);
}
