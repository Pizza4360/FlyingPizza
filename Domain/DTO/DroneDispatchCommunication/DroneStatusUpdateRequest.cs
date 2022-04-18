using System.Text.Json.Serialization;
using Domain.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.DTO.DroneDispatchCommunication
{
    public class DroneStatusUpdateRequest : BaseDto
    {
        [BsonId]
        [BsonElement("Id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id;    
        
        [BsonElement("Location"), JsonPropertyName("Location")]
        public GeoLocation Location { get; set; } 
        
        [BsonElement("State"), JsonPropertyName("State")]
        public DroneState State { get; set; }       
        
    }
}
