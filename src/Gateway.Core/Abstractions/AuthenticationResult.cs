namespace Gateway.Core.Abstractions;

/// <summary>
/// Result from authentication service
/// </summary>
public record AuthenticationResult(
    bool IsAuthenticated,
    bool IsAuthorized,
    IDictionary<string, string>? Claims = null,
    string? ErrorMessage = null
);