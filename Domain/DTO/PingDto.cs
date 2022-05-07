using Newtonsoft.Json;

namespace Domain.DTO;

public class PingDto
{
    [JsonProperty("S")] public string S { get; set; }
}