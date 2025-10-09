using Gateway.Auth.Middleware;
using Microsoft.AspNetCore.Builder;

namespace Gateway.Auth.Extensions;

public static class ApplicationBuilderExtensions
{
    public static IApplicationBuilder UseGatewayAuth(this IApplicationBuilder app)
    {
        return app.UseMiddleware<AuthMiddleware>();
    }
}
