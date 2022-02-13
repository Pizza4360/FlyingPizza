using Domain.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities
{
    public class Drone : IBaseEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string BadgeNumber { get; set; }

        public string OrderId { get; set; }

        public GeoLocation HomeLocation { get; set; }

        [BsonElement("Location")]
        public GeoLocation CurrentLocation { get; set; }

        public GeoLocation Destination { get; set; }

        public string Status { get; set; }

        public string IPAddress { get; set; }
    }
}
