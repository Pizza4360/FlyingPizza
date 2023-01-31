using System.Text.Json.Serialization;

namespace Domain.DTO.DroneDispatchCommunication;

public class CompleteDeliveryResponse : BaseDto
{
    [JsonPropertyName("IsAcknowledged")] public bool IsAcknowledged { get; set; }
}