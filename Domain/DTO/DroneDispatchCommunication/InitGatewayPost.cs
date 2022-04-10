using System.Text.Json.Serialization;
using Domain.Gateways;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.DTO.DroneDispatchCommunication
{
    public class InitGatewayPost : BaseDTO
    {
        [BsonElement("Url"), JsonPropertyName("Url")]
        public string Url { get; set; }
        
        [BsonElement("BadgeNumber"), JsonPropertyName("BadgeNumber")]
        public int BadgeNumber { get; set; }
    }
}
