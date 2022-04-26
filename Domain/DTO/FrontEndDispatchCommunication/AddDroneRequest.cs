using System;
using System.Text.Json.Serialization;
using MongoDB.Bson;

namespace Domain.DTO.FrontEndDispatchCommunication;

public class AddDroneRequest
{
    public string DroneId { get; set; }
    public Guid BadgeNumber { get; set; }
    public GeoLocation HomeLocation { get; set; }
    public string DroneUrl { get; set; }
    public string DispatchUrl { get; set; }
}
