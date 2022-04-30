using System.Text.Json.Serialization;

namespace Domain.DTO.DroneDispatchCommunication;

public class CompleteOrderResponse
{
    [JsonPropertyName("IsAcknowledged")] public bool IsAcknowledged { get; set; }
}