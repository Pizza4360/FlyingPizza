using System.Text.Json.Serialization;

namespace Domain.DTO.FrontEndDispatchCommunication;

public class CancelDeliveryRequest : BaseDto
{
    [JsonPropertyName("DeliveryId")] public string DeliveryId { get; set; }
}