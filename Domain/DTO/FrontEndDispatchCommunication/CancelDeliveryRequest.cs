using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.DTO.FrontEndDispatchCommunication;

public class CancelDeliveryRequest : BaseDto
{
    [BsonId]
    [BsonElement("OrderId")]
    [BsonRepresentation(BsonType.ObjectId)]
    public string OrderId;
}
