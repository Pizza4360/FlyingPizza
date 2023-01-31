using System.Text.Json.Serialization;

namespace Domain.DTO.DroneDispatchCommunication;

public class UpdateDroneStatusRequest
{
    [JsonPropertyName("DroneId")]
    public string DroneId{get;set;}

    [JsonPropertyName("Location")]
    public GeoLocation Location { get; set; } 
        
    [JsonPropertyName("Status")]
    public DroneStatus Status { get; set; }
}