using System;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Domain.DTO;

public class DeliveryUpdate
{
    [BsonElement("DeliveryId")]
    [JsonProperty("DeliveryId")]
    public string DeliveryId { get; set; }

    [BsonElement("Status")]
    [JsonProperty("Status")]
    public Deliveriestatus Status { get; set; }

    [BsonElement("TimeDelivered")]
    [JsonProperty("TimeDelivered")]
    public DateTime? TimeDelivered { get; set; }

    [BsonElement("HasBeenDelivered")]
    [JsonProperty("HasBeenDelivered")]
    public bool HasBeenDelivered { get; set; }

    [BsonElement("DroneId")]
    [JsonProperty("DroneId")]
    public string DroneId { get; set; }
}