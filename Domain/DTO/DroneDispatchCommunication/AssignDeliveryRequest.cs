using System.Text.Json.Serialization;
using Domain.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.DTO.DroneDispatchCommunication;

public class AssignDeliveryRequest
{
    [JsonPropertyName("OrderId")]
    public Order Order { get; set; } 
        
    [JsonPropertyName("DroneUrl")]
    public string DroneUrl { get; set; }
}