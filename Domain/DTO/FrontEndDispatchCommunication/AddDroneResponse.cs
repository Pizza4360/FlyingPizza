using System;
using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.DTO.FrontEndDispatchCommunication
{
    public class AddDroneResponse : BaseDto
    {
        [BsonElement("Success"), JsonPropertyName("Success")]
        public bool Success { get; set; }

        [BsonId]
        [BsonElement("DroneId")]
        [JsonPropertyName("DroneId")]
        public Guid BadgeNumber { get; set; }    
    }
}