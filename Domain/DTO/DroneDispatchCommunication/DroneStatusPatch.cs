using System.Text.Json.Serialization;
using Domain.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.DTO.DroneDispatchCommunication
{
    public class DroneStatusPatch : BaseDTO
    {
        [BsonElement("Location"), JsonPropertyName("Location")]
        public GeoLocation Location { get; set; } 
        
        [BsonElement("State"), JsonPropertyName("State")]
        public string State { get; set; }       
        
        [BsonId]
        [BsonElement("Id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id;    
    }
}
