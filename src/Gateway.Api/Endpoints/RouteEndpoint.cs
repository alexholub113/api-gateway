namespace Gateway.Api.Endpoints;

public class RouteEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        // Handle routes with remaining path: /route/{serviceId}/{downstream-path}
        app.MapMethods("/route/{serviceId}/{**downstreamPath}",
            ["GET", "POST", "PUT", "DELETE", "PATCH", "HEAD", "OPTIONS"],
            HandleAsync);
    }

    private static async Task HandleAsync(
        string serviceId,
        string downstreamPath,
        HttpContext context,
        IGatewayHandler gatewayHandler)
    {
        var result = await gatewayHandler.RouteRequestAsync(context, serviceId, downstreamPath);

        if (result.IsFailure)
        {
            // Only set response if proxy failed and response hasn't started
            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync(result.Error.Message);
            }
        }

        // If successful, response has already been written by ProxyHandler
    }
}
