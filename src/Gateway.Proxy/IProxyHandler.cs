using Gateway.Common.Models.Result;
using Gateway.ServiceRouting.Models;
using Microsoft.AspNetCore.Http;

namespace Gateway.Proxy;

public interface IProxyHandler
{
    Task<Result> ProxyRequestAsync(HttpContext context, RouteMatch routeMatch, Uri uri);
}
