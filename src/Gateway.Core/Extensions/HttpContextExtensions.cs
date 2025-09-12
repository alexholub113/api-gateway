using Gateway.Core.Abstractions;
using Gateway.Core.Models;
using Microsoft.AspNetCore.Http;

namespace Gateway.Core.Extensions;

/// <summary>
/// Extension methods for HttpContext to work with Gateway context
/// </summary>
public static class HttpContextExtensions
{
    private const string GatewayContextKey = "Gateway.Context";

    /// <summary>
    /// Gets the gateway context for the current request
    /// </summary>
    public static IGatewayContext GetGatewayContext(this HttpContext context)
    {
        if (!context.Items.TryGetValue(GatewayContextKey, out var ctx))
        {
            throw new InvalidOperationException("Gateway context not found. Ensure GatewayContextMiddleware is registered in the pipeline.");
        }
        return (IGatewayContext)ctx!;
    }

    /// <summary>
    /// Internal method to initialize the gateway context. Should only be called by gateway middleware.
    /// </summary>
    internal static void InitializeGatewayContext(this HttpContext context)
    {
        if (!context.Items.ContainsKey(GatewayContextKey))
        {
            var gatewayContext = new GatewayContext();
            context.Items[GatewayContextKey] = gatewayContext;
        }
    }
}