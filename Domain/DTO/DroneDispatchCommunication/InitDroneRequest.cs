using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.DTO.DroneDispatchCommunication
{
    public class InitDroneRequest : BaseDto
    {
        [BsonId]
        [BsonElement("DroneId")]
        [JsonPropertyName("DroneId")]
        public string DroneId { get; set; }
        
        [BsonElement("Ip")]
        [JsonPropertyName("Ip")]
        public string DroneIp { get; set; }
    }
}
