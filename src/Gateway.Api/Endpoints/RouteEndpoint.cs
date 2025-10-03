using Gateway.Common.Extensions;

namespace Gateway.Api.Endpoints;

public class RouteEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        // Handle routes with remaining path: /route/{serviceId}/{downstream-path}
        app.MapMethods("/route/{**downstreamPath}",
            ["GET", "POST", "PUT", "DELETE", "PATCH", "HEAD", "OPTIONS"],
            HandleAsync);
    }

    private static async Task HandleAsync(
        string downstreamPath,
        HttpContext context,
        IGatewayHandler gatewayHandler,
        ILogger<RouteEndpoint> logger)
    {
        var requestId = context.TraceIdentifier;

        var result = await gatewayHandler.RouteRequestAsync(context, downstreamPath);

        if (result.IsFailure)
        {
            var serviceId = context.GetGatewayTargetServiceId() ?? "unknown";
            logger.LogWarning("Request failed for service '{serviceId}' with error: {Error} (RequestId: {RequestId})",
                serviceId, result.Error.Message, requestId);

            // Only set response if proxy failed and response hasn't started
            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync(result.Error.Message);
            }
        }
    }
}
