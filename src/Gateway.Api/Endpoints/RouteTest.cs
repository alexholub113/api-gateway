using Gateway.Core.Extensions;
using MinimalEndpoints.Abstractions;

namespace Gateway.Api.Endpoints;

public record RouteTestResponse(
    string? MatchedRouteId,
    string? MatchedPattern,
    string? TargetService,
    string RequestPath,
    string RequestMethod
);

public class RouteTest : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/test/route", HandleAsync);
        app.MapPost("/test/route", HandleAsync);
    }

    private static ValueTask<RouteTestResponse> HandleAsync(HttpContext context)
    {
        var gatewayContext = context.GetGatewayContext();
        
        var response = new RouteTestResponse(
            MatchedRouteId: gatewayContext.RouteMatch?.RouteId,
            MatchedPattern: gatewayContext.RouteMatch?.Pattern,
            TargetService: gatewayContext.RouteMatch?.TargetServiceName,
            RequestPath: context.Request.Path.Value ?? "",
            RequestMethod: context.Request.Method
        );

        return ValueTask.FromResult(response);
    }
}