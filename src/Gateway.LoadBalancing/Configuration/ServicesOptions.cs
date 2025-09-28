namespace Gateway.LoadBalancing.Configuration;

/// <summary>
/// Configuration for backend services
/// </summary>
internal class ServicesOptions
{
    public const string SectionName = "Services";

    public List<ServiceDefinition> Services { get; set; } = [];
}