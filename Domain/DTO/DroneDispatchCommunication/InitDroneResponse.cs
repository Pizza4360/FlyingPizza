using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.DTO.DroneDispatchCommunication
{
    public class InitDroneResponse : BaseDTO
    {
        [BsonId]
        [BsonElement("Id")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        [BsonElement("Okay")]
        public bool Okay { get; set; }
    }
}
