using System.Text.Json.Serialization;
using Domain.DTO.FrontEndDispatchCommunication;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.DTO.DroneDispatchCommunication;

public class InitDroneRequest
{
    [JsonPropertyName("DroneId")]
    public string DroneId { get; set; }
        
    [JsonPropertyName("DroneIp")]
    public string DroneIp { get; set; }
}