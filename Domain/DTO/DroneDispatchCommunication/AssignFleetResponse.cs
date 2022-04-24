using System.Data;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Domain;

namespace Domain.DTO.DroneDispatchCommunication
{
    public class AssignFleetResponse : BaseDto
    {
        [BsonId]
        [BsonElement("DroneId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id;

        [BsonElement("IsInitializedAndAssigned")]
        public bool IsInitializedAndAssigned;
        
        public DroneState FirstState { get; set; }
    }
}
