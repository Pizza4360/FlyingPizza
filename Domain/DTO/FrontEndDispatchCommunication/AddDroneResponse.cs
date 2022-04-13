using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.DTO.FrontEndDispatchCommunication
{
    public class AddDroneResponse : BaseDTO
    {
        [BsonElement("Success"), JsonPropertyName("Success")]
        public bool Success { get; set; }

        [BsonId]
        [BsonElement("Id")]
        [JsonPropertyName("Id")]
        public int Id { get; set; }    
    }
}