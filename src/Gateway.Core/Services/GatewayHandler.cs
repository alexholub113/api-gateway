using Gateway.Common;
using Gateway.LoadBalancing;
using Gateway.Proxy;
using Gateway.ServiceRouting;
using Microsoft.AspNetCore.Http;

namespace Gateway.Core.Services;

internal class GatewayHandler(IRouteResolver routeResolver, ILoadBalancer loadBalancer, IProxyHandler proxyHandler) : IGatewayHandler
{
    public async ValueTask<Result> RouteRequestAsync(HttpContext context)
    {
        var routeResolverResult = routeResolver.ResolveRoute(
            context.Request.Path.Value ?? "",
            context.Request.Method
        );

        if (routeResolverResult.IsFailure)
        {
            return Result.Failure(routeResolverResult.Error);
        }

        var loadBalancerResult = loadBalancer.SelectInstance(routeResolverResult.Value.TargetServiceName);
        if (loadBalancerResult.IsFailure)
        {
            return Result.Failure(loadBalancerResult.Error);
        }

        var routeMatch = routeResolverResult.Value;
        var uri = loadBalancerResult.Value;
        var proxyResult = await proxyHandler.ProxyRequestAsync(context, routeMatch, uri);
        if (proxyResult.IsFailure)
        {
            return Result.Failure(proxyResult.Error);
        }

        return Result.Success();
    }
}
