using System.Text.Json.Serialization;
using Domain.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities
{
    public class DroneRecord : IBaseEntity
    {
        [BsonId, BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("BadgeNumber"), JsonPropertyName("BadgeNumber")]
        public int BadgeNumber { get; set; }
        
        [BsonElement("OrderId"), JsonPropertyName("OrderId")]
        public string OrderId { get; set; }
        
        [BsonElement("Destination"), JsonPropertyName("Destination")]
        public GeoLocation Destination { get; set; }
        
        [BsonElement("CurrentLocation"), JsonPropertyName("CurrentLocation")]
        public GeoLocation CurrentLocation { get; set; }

        [BsonElement("HomeLocation"), JsonPropertyName("HomeLocation")]
        public GeoLocation HomeLocation { get; set; }

        [BsonElement("State"), JsonPropertyName("State")]
        public string State { get; set; }

        
        [BsonElement("IpAddress"), JsonPropertyName("IpAddress")]
        public string IpAddress { get; set; }

        [BsonElement("DispatcherUrl"), JsonPropertyName("DispatcherUrl")]
        public string DispatcherUrl { get; set; }


        public override string ToString()
        {
            return $"ID:{Id}" +
                   $"Currentlocation:{CurrentLocation}\n" +
                   $"Destination:{Destination}\n" + 
                   $"Status:{State}";
        }

        public override bool Equals(object o)
        {
            if (o == null || o.GetType() != GetType()) return false;
            DroneRecord oo = (DroneRecord) o;
            return oo.BadgeNumber == BadgeNumber &&
                   oo.CurrentLocation.Equals(CurrentLocation) &&
                   oo.Destination.Equals(Destination) &&
                   oo.State.Equals(State) &&
                   oo.DispatcherUrl.Equals(DispatcherUrl);
        }
    }
}
