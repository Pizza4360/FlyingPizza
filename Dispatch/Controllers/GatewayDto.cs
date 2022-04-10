namespace Dispatch.Controllers;

public class GatewayDto
{
    public string Url { get; set; }
    public override string ToString() => $"{{Url: {Url}}}";
}