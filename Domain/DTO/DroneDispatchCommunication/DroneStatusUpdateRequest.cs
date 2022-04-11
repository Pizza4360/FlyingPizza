using System.Text.Json.Serialization;
using Domain.DTO.Shared;
using Domain.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.DTO.DroneDispatchCommunication
{
    public class DroneStatusUpdateRequest : BaseDTO
    {
        [BsonId]
        [BsonElement("Id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id;    
        
        [BsonElement("Location"), JsonPropertyName("Location")]
        public GeoLocation Location { get; set; } 
        
        [BsonElement("State"), JsonPropertyName("State")]
        public string State { get; set; }       
        
    }
}
