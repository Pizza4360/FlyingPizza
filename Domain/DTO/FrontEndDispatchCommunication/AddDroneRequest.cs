using System.Text.Json.Serialization;

namespace Domain.DTO.FrontEndDispatchCommunication;

public class AddDroneRequest
{
    [JsonPropertyName("DroneId")]
    public string DroneId { get; set; }
        
    [JsonPropertyName("BadgeNumber")]
    public Guid BadgeNumber { get; set; }    
        
    [JsonPropertyName("HomeLocation")]
    public GeoLocation HomeLocation { get; set; }
        
    [JsonPropertyName("DroneUrl")]
    public string DroneUrl { get; set; }

    [JsonPropertyName("DispatchUrl")]
    public string DispatchUrl { get; set; }
}
