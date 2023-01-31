using System.Text.Json.Serialization;

namespace Domain.DTO.FrontEndDispatchCommunication;

public class CancelDeliveryResponse : BaseDto
{
    [JsonPropertyName("DeliveryId")] public string DeliveryId { get; set; }

    [JsonPropertyName("IsCancelled")] public bool IsCancelled { get; set; }
}