using Gateway.Core.Abstractions;

namespace Gateway.Core.Configuration;

/// <summary>
/// Configuration for backend services
/// </summary>
public class ServicesOptions
{
    public const string SectionName = "Services";

    public List<ServiceDefinition> Services { get; set; } = new();
}