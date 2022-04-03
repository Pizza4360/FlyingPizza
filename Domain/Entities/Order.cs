using System;
using System.Collections.Generic;
using Domain.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities
{
    public class Order : IBaseEntity
    {
        [BsonElement]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public List<Object> Items {get; set;}
        
        public string CustomerName { get; set; }

        public string DeliveryAddress { get; set; }
        
        public GeoLocation DeliveryLocation { get; set; }
        
        public int BadgeNumber {get; set;}
        
        public DateTimeOffset TimeOrdered { get; set; }
        
        public DateTimeOffset? TimeDelivered { get; set; }
        
        public string URL { get; set; }

        public bool HasBeenDelivered
        { get { return (TimeDelivered != null); } }
    }
}
