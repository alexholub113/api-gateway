namespace Gateway.Core.Configuration;

internal class GatewayOptions
{
    public const string SectionName = "Gateway";

    public TargetServiceSettings[] TargetServices { get; set; } = [];
}
