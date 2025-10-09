using System.Diagnostics;
using Gateway.Auth.Services;
using Gateway.Auth.Telemetry;
using Microsoft.AspNetCore.Http;

namespace Gateway.Auth.Middleware;

internal class AuthMiddleware(
    RequestDelegate next,
    JwtValidationService validationService,
    AuthTelemetry telemetry,
    ILogger<AuthMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var serviceId = context.GetGatewayTargetServiceId() ?? "unknown";
        var serviceSettings = context.Items["GatewayServiceSettings"] as TargetServiceSettings;

        // Check if service has auth policy configured
        if (serviceSettings?.AuthPolicy == null)
        {
            await next(context);
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        var authPolicy = serviceSettings.AuthPolicy;
        var policyName = $"{serviceId}-auth";

        try
        {
            // Extract Bearer token
            var authHeader = context.Request.Headers.Authorization.ToString();
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
            {
                logger.LogWarning("Missing or invalid Authorization header for service '{ServiceId}'", serviceId);

                stopwatch.Stop();
                telemetry.RecordAuthRequest(serviceId, "missing_token", stopwatch.Elapsed.TotalMilliseconds, policyName);
                telemetry.RecordAuthError(serviceId, "missing_token", policyName);

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { error = "Authorization header is required" });
                return;
            }

            var token = authHeader["Bearer ".Length..].Trim();

            // Validate token
            var (isValid, errorMessage, subject) = await validationService.ValidateTokenAsync(token, authPolicy);

            stopwatch.Stop();

            if (!isValid)
            {
                logger.LogWarning("Token validation failed for service '{ServiceId}': {Error}", serviceId, errorMessage);

                telemetry.RecordAuthRequest(serviceId, "invalid_token", stopwatch.Elapsed.TotalMilliseconds, policyName);
                telemetry.RecordAuthError(serviceId, "invalid_token", policyName);

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { error = errorMessage ?? "Invalid token" });
                return;
            }

            // Store validated subject in context for downstream use
            context.Items["AuthenticatedUser"] = subject;

            telemetry.RecordAuthRequest(serviceId, "success", stopwatch.Elapsed.TotalMilliseconds, policyName);

            logger.LogDebug("Token validated successfully for service '{ServiceId}', user: {Subject}", serviceId, subject);

            await next(context);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            logger.LogError(ex, "Authentication middleware error for service '{ServiceId}'", serviceId);

            telemetry.RecordAuthRequest(serviceId, "error", stopwatch.Elapsed.TotalMilliseconds, policyName);
            telemetry.RecordAuthError(serviceId, "middleware_error", policyName);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new { error = "Authentication error" });
        }
    }
}
