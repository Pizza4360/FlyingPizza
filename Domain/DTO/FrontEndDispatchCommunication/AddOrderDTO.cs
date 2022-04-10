using System.Text.Json.Serialization;
using Domain.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.DTO.FrontEndDispatchCommunication
{
    public class AddOrderDTO : BaseDTO
    {
        [BsonId]
        [BsonElement("Id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ID;
        
        [BsonElement("DeliveryLocaion"), JsonPropertyName("DeliveryLocaion")]
        public GeoLocation DeliveryLocation { get; set; }
    }
}