using System.Text.Json.Serialization;
using Domain.DTO.FrontEndDispatchCommunication;
using Domain.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.DTO.DroneDispatchCommunication;

public class UpdateDroneStatusRequest
{
    public string DroneId{get;set;}
    public GeoLocation Location { get; set; }
    public DroneState State { get; set; }
}