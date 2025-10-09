namespace Gateway.Common.Configuration;

public class AuthPolicy
{
    public List<string> ValidIssuers { get; set; } = new();
    public List<string> ValidAudiences { get; set; } = new();
}
