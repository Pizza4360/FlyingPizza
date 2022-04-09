using System;
using System.Linq;
using System.Text.Json.Serialization;
using Domain.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Domain.Entities
{
    [BsonDiscriminator("Order")]
    public class Order
    {
        [BsonElement("Id")]
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ID;
        
        private static Random _random = new Random();
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        private const int IdLength = 24;
            
          
        // https://stackoverflow.com/questions/1344221/how-can-i-generate-random-alphanumeric-strings
        public static string GenerateNewID()
        {
            return new string(Enumerable.Repeat(chars, IdLength)
                .Select(s => s[_random.Next(s.Length)]).ToArray());
        }
        
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
        [JsonPropertyName("HasBeenDelivered")]
        [BsonIgnoreIfNull]
        public bool HasBeenDelivered => TimeDelivered != null;
    }
}

