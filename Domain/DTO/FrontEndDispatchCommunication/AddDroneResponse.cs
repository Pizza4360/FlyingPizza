using System;
using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.DTO.FrontEndDispatchCommunication
{
    public class AddDroneResponse: BaseDto
    {
        [JsonPropertyName("Success")]
        public bool Success { get; set; }

        [JsonPropertyName("BadgeNumber")]
        public Guid BadgeNumber { get; set; }    
    }
}