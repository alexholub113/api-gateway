namespace Gateway.Common.Configuration;

public class CachePolicy
{
    public TimeSpan Duration { get; set; }
    public List<string> Methods { get; set; } = new() { "GET" };
    public List<string> VaryByHeaders { get; set; } = new();
    public bool VaryByQuery { get; set; } = true;
    public bool VaryByUser { get; set; } = false;
}
