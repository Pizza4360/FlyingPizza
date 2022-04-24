using System;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Domain.DTO.FrontEndDispatchCommunication
{
    public class AddDroneRequest : BaseDto
    {
        [BsonElement("DroneId"), JsonPropertyName("DroneId")]
        public string DroneId { get; set; }
        
        [BsonElement("BadgeNumber"), JsonPropertyName("BadgeNumber")]
        [BsonId(IdGenerator = typeof(Guid))]
        public Guid BadgeNumber { get; set; }    
        
        [BsonElement("HomeLocation"), JsonPropertyName("HomeLocation")]
        public GeoLocation HomeLocation { get; set; }
        
        [BsonElement("DroneIp"), JsonPropertyName("DroneIp")]
        public string DroneIp { get; set; }
        
        [BsonElement("DispatchIp"), JsonPropertyName("DispatchIp")]
        public string DispatchIp { get; set; }        
        
    
    }
}
