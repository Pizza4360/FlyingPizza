using System;
using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.DTO.DroneDispatchCommunication;

public class CompleteOrderRequest : BaseDTO
{
    [BsonId]
    [BsonElement("Id")]
    [JsonPropertyName("OrderId")]
    public string OrderId { get; set; }
    
    [BsonElement("Time")]
    [JsonPropertyName("Time")]
    public DateTime Time { get; set; }
    
}
