using System;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Domain.DTO.FrontEndDispatchCommunication;

public class AddDroneRequest: BaseDto
{
    [JsonPropertyName("DroneId")]
    public string DroneId { get; set; }
        
    [JsonPropertyName("BadgeNumber")]
    public Guid BadgeNumber { get; set; }    
        
    [JsonPropertyName("HomeLocation")]
    public GeoLocation HomeLocation { get; set; }
        
    [JsonPropertyName("DroneIp")]
    public string DroneIp { get; set; }
        
    [JsonPropertyName("DispatchIp")]
    public string DispatchIp { get; set; }
}

public class BaseDto
{
    // public override string ToString() =>  this.ToJson();
}