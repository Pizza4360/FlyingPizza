using System;
using System.Text.Json.Serialization;
using Domain.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities
{
    public class Order : IBaseEntity
    {
        [BsonId, BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        [BsonElement("Items")]
        [JsonPropertyName("Items")]
        public object[] Items { get; set; }

        [BsonElement("CustomerName")]
        [JsonPropertyName("CustomerName")]
        public string CustomerName { get; set; }

        [BsonElement("DeliveryAddress")]
        [JsonPropertyName("DeliveryAddress")]
        public string DeliveryAddress { get; set; }

        [BsonElement("DeliveryLocation")]
        [JsonPropertyName("DeliveryLocation")]
        public GeoLocation DeliveryLocation { get; set; }

        [BsonElement("TimeOrdered")]
        [JsonPropertyName("TimeOrdered")]
        [BsonRepresentation(BsonType.DateTime)]
        public DateTime TimeOrdered { get; set; }

        [BsonElement("TimeDelivered")]
        [JsonPropertyName("TimeDelivered")]
        [BsonRepresentation(BsonType.DateTime)]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        [BsonIgnoreIfNull]
        public DateTime? TimeDelivered { get; set; }
        
        [BsonElement("Url")]
        [JsonPropertyName("Url")]
        public string URL { get; set; }
        
        [BsonElement("BadgeNumber")]
        [JsonPropertyName("BadgeNumber")]
        public int BadgeNumber { get; set; }

        [BsonElement("HasBeenDelivered")]
        [JsonPropertyName("HasBeenDelivered"), BsonIgnoreIfNull]
        public bool HasBeenDelivered => TimeDelivered != null;
    }
}
