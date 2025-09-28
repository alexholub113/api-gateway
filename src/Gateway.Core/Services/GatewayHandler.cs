using Gateway.Common.Extensions;
using Gateway.Common.Models.Result;
using Gateway.LoadBalancing;
using Gateway.Proxy;
using Gateway.ServiceRouting;
using Microsoft.AspNetCore.Http;

namespace Gateway.Core.Services;

internal class GatewayHandler(IRouteResolver routeResolver, ILoadBalancer loadBalancer, IProxyHandler proxyHandler) : IGatewayHandler
{
    public async ValueTask<Result> RouteRequestAsync(HttpContext context, string serviceId, string downstreamPath)
    {
        return await routeResolver.ResolveRoute(serviceId, context.Request.Method, downstreamPath)
            .Bind(routeMatch => loadBalancer.SelectInstance(routeMatch.ServiceId)
            .Map(uri => (routeMatch, uri)))
            .BindAsync(results => proxyHandler.ProxyRequestAsync(context, results.routeMatch, results.uri));
    }
}
