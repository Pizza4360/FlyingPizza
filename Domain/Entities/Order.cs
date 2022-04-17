using System;
using System.Linq;
using System.Text.Json.Serialization;
using Domain.DTO;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities
{
    [BsonDiscriminator("Order")]
    public class Order : BaseDto
    {
        // [BsonId]
        // [BsonElement("Id")]
        // [BsonRepresentation(BsonType.ObjectId)]
        // public string Id;

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
        // cqTFuSE656EQTy1jl8rqwiVB
        [BsonElement("BadgeNumber")]
        [JsonPropertyName("BadgeNumber")]
        [BsonRepresentation(BsonType.String)]
        public Guid BadgeNumber { get; set; }

        [BsonElement("HasBeenDelivered")]
        [JsonPropertyName("HasBeenDelivered")]
        [BsonIgnoreIfNull]
        public bool HasBeenDelivered => TimeDelivered != null;
    }
}

