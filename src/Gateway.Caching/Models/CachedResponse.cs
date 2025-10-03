namespace Gateway.Caching.Models;

/// <summary>
/// Cached HTTP response data
/// </summary>
public record CachedResponse
{
    public int StatusCode { get; init; }
    public Dictionary<string, string[]> Headers { get; init; } = [];
    public byte[] Body { get; init; } = [];
    public string ContentType { get; init; } = string.Empty;
    public DateTime CachedAt { get; init; } = DateTime.UtcNow;
}