using System;
using System.Text.Json.Serialization;
using Domain.DTO;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

public class DroneEntity : BaseEntity<DroneUpdate>
{
    public static string File() => "drone_data/DroneModel.json";

    [BsonElement("DeliveryId")]
    [JsonPropertyName("DeliveryId")]
    public string DeliveryId { get; set; }

    [BsonElement("BadgeNumber")]
    [JsonPropertyName("BadgeNumber")]
    public int BadgeNumber { get; set; }

    [BsonElement("Destination")]
    [JsonPropertyName("Destination")]
    public GeoLocation Destination { get; set; }

    [BsonElement("CurrentLocation")]
    [JsonPropertyName("CurrentLocation")]
    public GeoLocation CurrentLocation { get; set; }

    [BsonElement("HomeLocation")]
    [JsonPropertyName("HomeLocation")]
    public GeoLocation HomeLocation { get; set; }

    [BsonElement("Status")]
    [JsonPropertyName("Status")]
    public DroneStatus LatestStatus { get; set; }

    [BsonElement("DroneId")] public string DroneId { get; set; }

    [BsonElement("DroneUrl")]
    [JsonPropertyName("DroneUrl")]
    public string DroneUrl { get; set; }

    [BsonElement("DispatchUrl")]
    [JsonPropertyName("DispatchUrl")]
    public string DispatchUrl { get; set; }
    
    [BsonElement("BearingInDegrees")]
    [JsonPropertyName("BearingInDegrees")]
    public decimal BearingInDegrees { get; set; }

    public override DroneUpdate Update()
    {
        return new()
        {
            CurrentLocation = CurrentLocation,
            Destination = Destination,
            DroneId = DroneId,
            DeliveryId = DeliveryId,
            Status = LatestStatus
        };
    }

    public override string ToString()
    {
        return $"Id:{DroneId}" +
               $"Currentlocation:{CurrentLocation.ToCsv()}\n" +
               $"Destination:{Destination.ToCsv()}\n" +
               $"Status:{LatestStatus}" +
               $"Delivery Id:{DeliveryId}" +
               $"Home Location:{HomeLocation.ToCsv()}";
    }

    public override bool Equals(object o)
    {
        if (o == null ||
            o.GetType() != GetType()) return false;
        var oo = (DroneEntity) o;
        return oo.CurrentLocation.Equals(CurrentLocation) &&
               oo.Destination.Equals(Destination) &&
               oo.LatestStatus.Equals(LatestStatus) &&
               oo.DispatchUrl.Equals(DispatchUrl);
    }

    public object Display()
    {
        return $"Drone #{BadgeNumber}:\n" +
               $"Id:{DroneId}" +
               $"Currentlocation:{CurrentLocation}\n" +
               $"Destination:{Destination}\n" +
               $"Status:{LatestStatus}";
    }
}