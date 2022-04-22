using System;
using System.Text.Json.Serialization;
using Domain.DTO;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities
{
    public class DroneRecord : BaseEntity
    {

        [BsonElement("BadgeNumber")]
        [JsonPropertyName("BadgeNumber")]
        public Guid BadgeNumber { get; set; }


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


        [BsonElement("DroneIp")]
        [JsonPropertyName("DroneIp")]
        public string DroneIp { get; set; }

        [BsonElement("DispatchIp")]
        [JsonPropertyName("DispatchIp")]
        public string DispatchIp { get; set; }

        public override string ToString()
        {
            return $"DroneId:{DroneId}" +
                   $"Currentlocation:{CurrentLocation}\n" +
                   $"Destination:{Destination}\n" +
                   $"Status:{State}";
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
                   oo.DispatchIp.Equals(DispatchIp);
        }
    }
}
