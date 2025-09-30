namespace Gateway.Common.Configuration;

public class GatewayOptions
{
    public const string SectionName = "Gateway";

    public TargetServiceSettings[] TargetServices { get; set; } = [];
}
