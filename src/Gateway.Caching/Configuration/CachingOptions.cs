namespace Gateway.Caching.Configuration;

public class CachingOptions
{
    public const string SectionName = "Gateway:Caching";

    public Dictionary<string, CachePolicy> Policies { get; set; } = new();
    public string DefaultProvider { get; set; } = "Memory";
    public bool EnableCaching { get; set; } = true;
    public MemoryCacheSettings Memory { get; set; } = new();
}