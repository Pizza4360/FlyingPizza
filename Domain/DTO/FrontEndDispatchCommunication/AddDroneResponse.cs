using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.DTO.FrontEndDispatchCommunication
{
    public class AddDroneResponse : BaseDTO
    {
        [BsonElement("Success"), JsonPropertyName("Success")]
        public bool Success { get; set; }

        [BsonElement("BadgeNumber"), JsonPropertyName("BadgeNumber")]
        public int BadgeNumber { get; set; }    }
}