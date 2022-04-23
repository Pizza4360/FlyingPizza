using System.Text.Json.Serialization;

namespace Domain.DTO.DroneDispatchCommunication;

public class AssignFleetRequest
{
    [JsonPropertyName("DroneId")]
    public string DroneId { get; set; }

    [JsonPropertyName("DroneIp")]
    public string DroneIp {get;set;}
            
    [JsonPropertyName("DispatchIp")]
    public string DispatchIp{get;set;}

    [JsonPropertyName("BadgeNumber")]
    public Guid BadgeNumber { get; set; }    
            
    [JsonPropertyName("HomeLocation")]
    public GeoLocation HomeLocation { get; set; }
}