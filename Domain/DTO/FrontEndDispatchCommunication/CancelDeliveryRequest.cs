using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.DTO.FrontEndDispatchCommunication;

public class CancelDeliveryRequest: BaseDto
{
    [JsonPropertyName("OrderId")]
    public string OrderId;
}
