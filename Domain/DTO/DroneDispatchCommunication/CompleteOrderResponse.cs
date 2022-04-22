using System.Text.Json.Serialization;
using Domain.DTO.FrontEndDispatchCommunication;

namespace Domain.DTO.DroneDispatchCommunication;

public class CompleteOrderResponse: BaseDto
{
    [JsonPropertyName("IsAcknowledged")]
    public bool IsAcknowledged { get; set; }
}
