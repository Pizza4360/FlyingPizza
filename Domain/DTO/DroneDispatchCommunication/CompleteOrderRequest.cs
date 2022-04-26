using System;
using System.Text.Json.Serialization;
using Domain.DTO.FrontEndDispatchCommunication;
using Domain.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.DTO.DroneDispatchCommunication;

public class CompleteOrderRequest 
{
    public string OrderId { get; set; }
    public string DroneId { get; set; }
    public DateTime Time { get; set; }
    
}
