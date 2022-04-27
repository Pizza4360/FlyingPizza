using System;
using System.Text.Json.Serialization;
using MongoDB.Bson;

namespace Domain.DTO.DroneDispatchCommunication;

public class AssignFleetRequest
{
    public string DroneId { get; set; }
    public string DroneIp {get;set;}
    public string DispatchUrl{get;set;}
    public GeoLocation HomeLocation { get; set; }
}
