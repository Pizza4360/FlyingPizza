using System.Text.Json.Serialization;
using Domain.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.DTO.DroneDispatchCommunication;

public class AssignDeliveryRequest
{
    public Order Order { get; set; }
    public string DroneUrl { get; set; }
    public string DroneId{get;set;}
}