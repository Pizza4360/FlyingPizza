using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.DTO.DroneDispatchCommunication
{
    public class InitDroneRequest : BaseDto
    {
        [BsonElement("Url"), JsonPropertyName("Url")]
        public string Url { get; set; }
        
        [BsonElement("BadgeNumber"), JsonPropertyName("BadgeNumber")]
        public int BadgeNumber { get; set; }
    }
}
