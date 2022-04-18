using System.Data;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Domain;

namespace Domain.DTO.DroneDispatchCommunication
{
    public class AssignFleetResponse : BaseDto
    {
        [BsonId]
        [BsonElement("Id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id;

        [BsonElement("Okay")]
        public bool Okay;
        
        public DroneState FirstState { get; set; }
    }
}
