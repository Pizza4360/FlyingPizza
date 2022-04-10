using System;
using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.DTO.FrontEndDispatchCommunication
{
    public class AddDroneRequest : BaseDTO
    {
        [BsonElement("IpAddress"), JsonPropertyName("IpAddress")]
        public string IpAddress { get; set; }        
        
        [BsonElement("BadgeNumber"), JsonPropertyName("BadgeNumber")]
        public int BadgeNumber { get; set; }    }
}
