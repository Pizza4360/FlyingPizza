using System.Text.Json.Serialization;
using Domain.DTO.FrontEndDispatchCommunication;
using MongoDB.Bson;

namespace Domain.DTO.DroneDispatchCommunication;

public class InitDroneResponse
{
    [JsonPropertyName("DroneUrl")]
    public string DroneId { get; set; }
        
    [JsonPropertyName("Okay")]
    public bool Okay { get; set; }
}