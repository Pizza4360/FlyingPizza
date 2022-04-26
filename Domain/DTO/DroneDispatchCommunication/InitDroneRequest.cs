using System.Text.Json.Serialization;
using Domain.DTO.FrontEndDispatchCommunication;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.DTO.DroneDispatchCommunication;

public class InitDroneRequest
{
    [JsonPropertyName("DroneUrl")]
    public string DroneId { get; set; }
        
    [JsonPropertyName("DroneUrl")]
    public string DroneUrl { get; set; }
}