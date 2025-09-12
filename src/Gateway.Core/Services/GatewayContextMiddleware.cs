using Gateway.Core.Extensions;
using Microsoft.AspNetCore.Http;

namespace Gateway.Core.Services;

/// <summary>
/// Middleware that initializes the gateway context for each request
/// </summary>
internal class GatewayContextMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        // Initialize the gateway context for this request
        context.InitializeGatewayContext();

        await next(context);
    }
}