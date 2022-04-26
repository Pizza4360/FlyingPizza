using System.Text.Json.Serialization;

namespace Domain.DTO.FrontEndDispatchCommunication;

public class CancelDeliveryResponse
{
    [JsonPropertyName("Orders")]
    public string OrderId { get; set; }
    
    [JsonPropertyName("IsCancelled")]
    public bool IsCancelled { get; set; }
}
