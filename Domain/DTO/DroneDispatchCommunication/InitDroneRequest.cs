using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.DTO.DroneDispatchCommunication
{
    public class InitDroneRequest : BaseDto
    {
        [BsonId]
        [BsonElement("Id")]
        [JsonPropertyName("Id")]
        public string Id { get; set; }
        
        [BsonElement("Ip")]
        [JsonPropertyName("Ip")]
        public string DroneIp { get; set; }
    }
}
