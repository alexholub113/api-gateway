using Gateway.Core.Extensions;
using MinimalEndpoints.Abstractions;

namespace Gateway.Api.Endpoints;

public record GatewayStatusResponse(
    string RequestId,
    DateTime Timestamp,
    RequestMetricsResponse Metrics,
    string? RouteMatch,
    string? SelectedInstance,
    bool IsCircuitOpen,
    bool HasAuthResult,
    int PropertiesCount
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
        var gatewayContext = context.GetGatewayContext();

        var response = new GatewayStatusResponse(
            RequestId: context.TraceIdentifier,
            Timestamp: DateTime.UtcNow,
            Metrics: new RequestMetricsResponse(
                StartTime: gatewayContext.Metrics.StartTime,
                Duration: gatewayContext.Metrics.Duration,
                StatusCode: gatewayContext.Metrics.StatusCode,
                RequestSize: gatewayContext.Metrics.RequestSize,
                ResponseSize: gatewayContext.Metrics.ResponseSize
            ),
            RouteMatch: gatewayContext.RouteMatch?.RouteId,
            SelectedInstance: gatewayContext.SelectedInstance?.Url,
            IsCircuitOpen: gatewayContext.IsCircuitOpen,
            HasAuthResult: gatewayContext.AuthResult != null,
            PropertiesCount: gatewayContext.Properties.Count
        );

        return ValueTask.FromResult(response);
    }
}