using System.Text.Json.Serialization;
using Domain.DTO.FrontEndDispatchCommunication;

namespace Domain.DTO.DroneDispatchCommunication;

public class InitDroneResponse: BaseDto
{
    [JsonPropertyName("DroneId")]
    public string DroneId { get; set; }
        
    [JsonPropertyName("Okay")]
    public bool Okay { get; set; }
}