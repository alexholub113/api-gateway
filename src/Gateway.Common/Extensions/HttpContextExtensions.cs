using Microsoft.AspNetCore.Http;

namespace Gateway.Common.Extensions;
public static class HttpContextExtensions
{
    public static string? GetGatewayTargetServiceId(this HttpContext context)
    {
        if (context.Request.Headers.TryGetValue(GatewayHeaders.TargetServiceId, out var serviceId))
        {
            return serviceId.ToString();
        }

        return null;
    }
}
