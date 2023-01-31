using System;
using System.Text.Json.Serialization;
using Domain.DTO;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

[BsonDiscriminator("Delivery")]
public class DeliveryEntity : BaseEntity<DeliveryUpdate>
{
    [BsonElement("DroneId")]
    [JsonPropertyName("DroneId")]
    public string DroneId { get; set; }

    [BsonElement("DeliveryId")]
    [JsonPropertyName("DeliveryId")]
    public string DeliveryId { get; set; }

    [BsonElement("Status")]
    [JsonPropertyName("Status")]
    public Deliveriestatus Status { get; set; }

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

    [BsonElement("TimeDeliveryed")]
    [JsonPropertyName("TimeDeliveryed")]
    [BsonRepresentation(BsonType.DateTime)]
    public DateTime TimeOrdered { get; set; }

    [BsonIgnoreIfNull]
    [BsonRepresentation(BsonType.DateTime)]
    [BsonElement("TimeDelivered")]
    [JsonPropertyName("TimeDelivered")]
    public DateTime? TimeDelivered { get; set; }


    [BsonIgnoreIfNull]
    [BsonElement("HasBeenDelivered")]
    [JsonPropertyName("HasBeenDelivered")]
    public bool HasBeenDelivered => TimeDelivered != null;

    public override DeliveryUpdate Update()
    {
        return new DeliveryUpdate
        {
            DroneId = DroneId,
            HasBeenDelivered = HasBeenDelivered,
            DeliveryId = DeliveryId,
            Status = Status,
            TimeDelivered = TimeDelivered
        };
    }
}