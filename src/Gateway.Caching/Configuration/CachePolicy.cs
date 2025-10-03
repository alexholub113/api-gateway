namespace Gateway.Caching.Configuration;

public record CachePolicy(
    TimeSpan Duration,
    string[]? Methods = null,
    string[]? VaryByHeaders = null,
    bool VaryByQuery = true,
    bool VaryByUser = false)
{
    public static CachePolicy Create(TimeSpan duration, params string[] methods)
        => new(duration, methods ?? ["GET"]);

    public static CachePolicy CreateShort(int minutes = 1)
        => new(TimeSpan.FromMinutes(minutes), ["GET"]);

    public static CachePolicy CreateLong(int minutes = 30)
        => new(TimeSpan.FromMinutes(minutes), ["GET"], VaryByQuery: false);
}