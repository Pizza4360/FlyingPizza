using System.Text.Json.Serialization;
using Domain.DTO.FrontEndDispatchCommunication;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.DTO.DroneDispatchCommunication;

public class InitDroneRequest
{
    [JsonPropertyName("DroneId")]
    public string DroneId { get; set; }
        
    [JsonPropertyName("DroneUrl")]
    public string DroneUrl { get; set; }
}