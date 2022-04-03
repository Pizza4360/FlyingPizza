using System;
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

        public Guid BadgeNumber { get; set; }
        
        public GeoLocation CurrentLocation { get; set; }
        
        public GeoLocation Destination { get; set; }

        public string OrderId { get; set; }

        public string State { get; set; }
        
        public GeoLocation HomeLocation { get; set; }
        
        public string IpAddress { get; set; }

        public string DispatcherUrl { get; set; }
    }
}
