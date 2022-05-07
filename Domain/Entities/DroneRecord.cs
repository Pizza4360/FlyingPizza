using System;
using System.Text.Json.Serialization;
using Domain.DTO;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

public class DroneRecord : BaseEntity
{
    public static string File() => "drone_data/DroneRecord.json";

    [BsonElement("OrderId")]
    [JsonPropertyName("OrderId")]
    public string OrderId { get; set; }

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

    [BsonElement("DroneId")] public string DroneId { get; set; }

    [BsonElement("DroneUrl")]
    [JsonPropertyName("DroneUrl")]
    public string DroneUrl { get; set; }

    [BsonElement("DispatchUrl")]
    [JsonPropertyName("DispatchUrl")]
    public string DispatchUrl { get; set; }

    public DroneUpdate Update()
    {
        return new()
        {
            CurrentLocation = CurrentLocation,
            Destination = Destination,
            DroneId = DroneId,
            OrderId = OrderId,
            State = State
        };
    }

    public override string ToString()
    {
        return $"Id:{DroneId}" +
               $"Currentlocation:{CurrentLocation}\n" +
               $"Destination:{Destination}\n" +
               $"Status:{State}";
    }

    public override bool Equals(object o)
    {
        if (o == null ||
            o.GetType() != GetType()) return false;
        var oo = (DroneRecord) o;
        return oo.CurrentLocation.Equals(CurrentLocation) &&
               oo.Destination.Equals(Destination) &&
               oo.State.Equals(State) &&
               oo.DispatchUrl.Equals(DispatchUrl);
    }
}