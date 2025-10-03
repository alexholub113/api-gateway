namespace Gateway.Caching.Models;

/// <summary>
/// Result of a cache lookup operation
/// </summary>
public record CacheResult<T>
{
    public bool IsHit { get; init; }
    public T? Value { get; init; }
    public DateTime? ExpiresAt { get; init; }
    public string Key { get; init; } = string.Empty;

    public static CacheResult<T> Hit(T value, DateTime expiresAt, string key) 
        => new() { IsHit = true, Value = value, ExpiresAt = expiresAt, Key = key };
    
    public static CacheResult<T> Miss(string key) 
        => new() { IsHit = false, Key = key };
}