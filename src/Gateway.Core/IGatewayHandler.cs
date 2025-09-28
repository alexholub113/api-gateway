namespace Gateway.Core;

public interface IGatewayHandler
{
    ValueTask<Result> RouteRequestAsync(HttpContext context, string serviceId, string downstreamPath);
}
