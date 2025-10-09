using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Gateway.Auth.Services;

internal class JwtValidationService(ILogger<JwtValidationService> logger)
{
    private readonly JwtSecurityTokenHandler _tokenHandler = new();

    public async Task<(bool IsValid, string? ErrorMessage, string? Subject)> ValidateTokenAsync(
        string token,
        AuthPolicy policy)
    {
        try
        {
            var validationParameters = CreateValidationParameters(policy);

            var principal = await Task.Run(() =>
                _tokenHandler.ValidateToken(token, validationParameters, out _));

            var subject = principal.Identity?.Name ?? principal.FindFirst("sub")?.Value;

            logger.LogDebug("Token validated successfully for subject: {Subject}", subject);
            return (true, null, subject);
        }
        catch (SecurityTokenExpiredException)
        {
            logger.LogWarning("Token expired");
            return (false, "Token has expired", null);
        }
        catch (SecurityTokenInvalidSignatureException)
        {
            logger.LogWarning("Invalid token signature");
            return (false, "Invalid token signature", null);
        }
        catch (SecurityTokenInvalidIssuerException)
        {
            logger.LogWarning("Invalid token issuer");
            return (false, "Invalid token issuer", null);
        }
        catch (SecurityTokenInvalidAudienceException)
        {
            logger.LogWarning("Invalid token audience");
            return (false, "Invalid token audience", null);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Token validation failed");
            return (false, "Token validation failed", null);
        }
    }

    private TokenValidationParameters CreateValidationParameters(AuthPolicy policy)
    {
        var parameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = false,
            SignatureValidator = (token, parameters) => new JwtSecurityToken(token),
            RequireExpirationTime = false,
            ValidateLifetime = false
        };

        if (policy.ValidIssuers.Count > 0)
        {
            parameters.ValidIssuers = policy.ValidIssuers;
            parameters.ValidateIssuer = true;
        }
        else
        {
            parameters.ValidateIssuer = false;
        }

        if (policy.ValidAudiences.Count > 0)
        {
            parameters.ValidAudiences = policy.ValidAudiences;
            parameters.ValidateAudience = true;
        }
        else
        {
            parameters.ValidateAudience = false;
        }

        return parameters;
    }
}
