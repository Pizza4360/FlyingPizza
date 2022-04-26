using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Domain.DTO;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

public class DroneRecord
{
    [BsonElement("DroneId")]
    [JsonPropertyName("DroneId")]
    public string DroneId{get;set;}

    [BsonElement("Orders")]
    [JsonPropertyName("Orders")]
    public List<Order> Orders{get;set;}
    [BsonElement("BadgeNumber")]
    [JsonPropertyName("BadgeNumber")]
    public Guid BadgeNumber { get; set; }
    [BsonElement("Destination")]
    [JsonPropertyName("Destination")]
    public GeoLocation Destination { get; set; }
    [BsonElement("CurrentLocation")]
    [JsonPropertyName("CurrentLocation")]
    public GeoLocation CurrentLocation { get; set; }
    [BsonElement("HomeLocation")]
    [JsonPropertyName("HomeLocation")]
    public GeoLocation HomeLocation { get; set; }
    [BsonElement("State")]
    [JsonPropertyName("State")]
    public DroneState State { get; set; }
    [BsonElement("DroneUrl")]
    [JsonPropertyName("DroneUrl")]
    public string DroneUrl { get; set; }
    [BsonElement("DispatchUrl")]
    [JsonPropertyName("DispatchUrl")]
    public string DispatchUrl { get; set; }
    public override string ToString()
    {
        return this.ToJson();
    }
    public override bool Equals(object o)
    {
        if (o == null ||
            o.GetType() != GetType()) return false;
        DroneRecord oo = (DroneRecord) o;
        return oo.BadgeNumber == BadgeNumber &&
               oo.CurrentLocation.Equals(CurrentLocation) &&
               oo.Destination.Equals(Destination) &&
               oo.State.Equals(State) &&
               oo.DispatchUrl.Equals(DispatchUrl);
    }
}