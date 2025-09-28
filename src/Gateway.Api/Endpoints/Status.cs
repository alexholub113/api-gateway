using MinimalEndpoints.Abstractions;

namespace Gateway.Api.Endpoints;

public record GatewayStatusResponse(
    string RequestId
);

public record RequestMetricsResponse(
    DateTime StartTime,
    TimeSpan? Duration,
    int? StatusCode,
    long? RequestSize,
    long? ResponseSize
);

public class Status : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("/status", HandleAsync);
    }

    private static ValueTask<GatewayStatusResponse> HandleAsync(HttpContext context)
    {
        return ValueTask.FromResult(new GatewayStatusResponse("adasd"));
    }
}