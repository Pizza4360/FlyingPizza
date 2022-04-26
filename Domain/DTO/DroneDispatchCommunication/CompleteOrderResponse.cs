using System.Text.Json.Serialization;

namespace Domain.DTO.DroneDispatchCommunication;

public class CompleteOrderResponse
{
    public bool IsAcknowledged { get; set; }
}
