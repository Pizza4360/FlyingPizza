using System;
using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.DTO.FrontEndDispatchCommunication
{
    public class AddDroneRequest : BaseDTO
    {
        [BsonElement("Id"), JsonPropertyName("Id")]
        public string Id { get; set; }
        
        [BsonElement("BadgeNumber"), JsonPropertyName("BadgeNumber")]
        public Guid BadgeNumber { get; set; }    
        
        [BsonElement("HomeLocation"), JsonPropertyName("HomeLocation")]
        public GeoLocation HomeLocation { get; set; }
        
        [BsonElement("DroneIp"), JsonPropertyName("DroneIp")]
        public string DroneIp { get; set; }
        
        [BsonElement("DispatchIp"), JsonPropertyName("DispatchIp")]
        public string DispatchIp { get; set; }        
        
    
    }
}
