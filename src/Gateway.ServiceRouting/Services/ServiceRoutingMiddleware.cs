using Gateway.Core.Extensions;
using Gateway.ServiceRouting.Abstractions;
using Microsoft.AspNetCore.Http;

namespace Gateway.ServiceRouting.Services;

/// <summary>
/// Middleware that resolves routes for incoming requests
/// </summary>
internal class ServiceRoutingMiddleware(RequestDelegate next, IRouteResolver routeResolver)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var gatewayContext = context.GetGatewayContext();

        // Resolve route for the incoming request
        var routeResult = routeResolver.ResolveRoute(
            context.Request.Path.Value ?? "",
            context.Request.Method
        );

        if (!routeResult.IsFailure)
        {
            // Store route match in gateway context for other middleware
            gatewayContext.RouteMatch = routeResult.Value;
        }

        await next(context);
    }
}