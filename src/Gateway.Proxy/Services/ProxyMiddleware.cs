using Gateway.Core.Extensions;
using Microsoft.AspNetCore.Http;

namespace Gateway.Proxy.Services;

/// <summary>
/// Middleware that handles proxying requests to target services
/// </summary>
internal class ProxyMiddleware(RequestDelegate next, HttpProxyService proxyService)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var gatewayContext = context.GetGatewayContext();

        // Only proxy if we have a route match and selected instance
        if (gatewayContext.RouteMatch != null && gatewayContext.SelectedInstance != null)
        {
            var result = await proxyService.ProxyRequestAsync(context, gatewayContext);

            if (!result.IsSuccess)
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsync(result.Error ?? "Service Unavailable");
                return;
            }

            // Request has been proxied, don't continue the pipeline
            return;
        }

        // No route match or service instance, continue to next middleware
        await next(context);
    }
}