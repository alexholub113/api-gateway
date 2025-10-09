namespace Gateway.Caching.Configuration;

public class CachingOptions
{
    public const string SectionName = "Gateway:Caching";

    public string DefaultProvider { get; set; } = "Memory";
    public bool EnableCaching { get; set; } = true;
    public MemoryCacheSettings Memory { get; set; } = new();
}