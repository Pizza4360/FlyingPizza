using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.DTO.DroneDispatchCommunication
{
    public class DroneStatusUpdateResponse : BaseDto
    {
        [BsonId]
        [BsonElement("DroneId")]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id;
    }
}
