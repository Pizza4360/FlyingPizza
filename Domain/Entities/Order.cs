using System;
using System.Text.Json.Serialization;
using Domain.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Domain.Entities
{
    [BsonDiscriminator("Order")]
    public class Order : BaseEntity
    {
        
        
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
    [BsonDiscriminator("user")]
    public class BrdUser
    {
        [BsonId(IdGenerator = typeof(StringObjectIdGenerator))]
        public string ID { get; set; }

        [BsonElement("username")]
        public string UserNm { get; set; }

        [BsonElement("email")]
        public string EmailAdrs { get; set; }

        public void get()
        {
            var o = new Order
            {
                CustomerName = "malc",
                DeliveryAddress = "444 some place",
                DeliveryLocation = new GeoLocation
                {
                    Latitude = 39.743787586026905m,
                    Longitude = -105.00333787196135m
                },
                Id = BaseEntity.GenerateNewID(),
                Items = new object[2],
                TimeOrdered = DateTime.Now,
                URL = "https://blah",
                BadgeNumber = -1,
                TimeDelivered = null
            };

            var bsonDocument = o.ToBsonDocument();
            var jsonDocument = bsonDocument.ToJson();

            Console.WriteLine ($"{bsonDocument}\n\n{jsonDocument}");
        }
    }
    
}

