﻿using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.DTO.DroneDispatchCommunication;

public class AssignDeliveryRequest
{
    [JsonPropertyName("OrderId")]
    public string OrderId { get; set; } 
    [JsonPropertyName("OrderLocation")]
    public GeoLocation OrderLocation { get; set; }
        
    [JsonPropertyName("DroneId")]
    public string DroneId { get; set; }
}