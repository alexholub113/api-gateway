using Gateway.Core;
using MinimalEndpoints.Abstractions;

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

    private static async ValueTask<IResult> HandleAsync(
        string serviceId,
        string downstreamPath,
        HttpContext context,
        IGatewayHandler gatewayHandler)
    {
        var result = await gatewayHandler.RouteRequestAsync(context, serviceId, downstreamPath);

        if (result.IsFailure)
        {
            return Results.BadRequest(result.Error);
        }

        return Results.Ok();
    }
}
