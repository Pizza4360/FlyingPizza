using System.Text.Json.Serialization;
using Domain.Entities;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.DTO.DroneDispatchCommunication
{
    public class AssignDeliveryRequest : BaseDTO
    {
        [BsonElement("OrderId"), JsonPropertyName("OrderId")]
        public string OrderId { get; set; } 
        [BsonElement("OrderLocation"), JsonPropertyName("OrderLocation")]
        public GeoLocation OrderLocation { get; set; }
        
        public string DroneId { get; set; }
    }
}
