using System.Security.Cryptography;
using System.Text;

namespace Gateway.Caching.Models;

/// <summary>
/// Represents a cache key with all the factors that affect cache uniqueness
/// </summary>
public record CacheKey
{
    public string ServiceId { get; init; } = string.Empty;
    public string Method { get; init; } = string.Empty;
    public string Path { get; init; } = string.Empty;
    public string QueryString { get; init; } = string.Empty;
    public Dictionary<string, string> Headers { get; init; } = new();
    public string UserId { get; init; } = string.Empty;

    /// <summary>
    /// Generates a deterministic cache key string
    /// </summary>
    public string ToKeyString()
    {
        var keyBuilder = new StringBuilder();
        keyBuilder.Append($"{ServiceId}:{Method}:{Path}");
        
        if (!string.IsNullOrEmpty(QueryString))
        {
            keyBuilder.Append($"?{QueryString}");
        }
        
        if (Headers.Any())
        {
            var sortedHeaders = Headers.OrderBy(h => h.Key);
            foreach (var header in sortedHeaders)
            {
                keyBuilder.Append($"|{header.Key}={header.Value}");
            }
        }
        
        if (!string.IsNullOrEmpty(UserId))
        {
            keyBuilder.Append($"|user={UserId}");
        }
        
        var key = keyBuilder.ToString();
        
        // Hash the key if it's too long for better performance
        if (key.Length > 250)
        {
            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(key));
            return Convert.ToBase64String(hashBytes);
        }
        
        return key;
    }
}