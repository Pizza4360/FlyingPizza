using System.Text.Json.Serialization;

namespace Domain.DTO.FrontEndDispatchCommunication;

public class CancelDeliveryRequest
{
    [JsonPropertyName("Orders")]
    public string OrderId{get;set;}
}
