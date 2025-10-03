namespace Gateway.Caching.Configuration;

public class MemoryCacheSettings
{
    public int SizeLimit { get; set; } = 1000;
    public TimeSpan DefaultExpiration { get; set; } = TimeSpan.FromMinutes(5);
}