﻿using System;
using System.Text.Json.Serialization;
using Domain.DTO.FrontEndDispatchCommunication;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.DTO.DroneDispatchCommunication;

public class CompleteOrderRequest 
{
    [JsonPropertyName("OrderId")]
    public string OrderId { get; set; }
    
    [JsonPropertyName("DroneId")]
    public string DroneId { get; set; }
    
    [JsonPropertyName("Time")]
    public DateTime Time { get; set; }
    
}
