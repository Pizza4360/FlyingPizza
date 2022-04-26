using System.Text.Json.Serialization;

namespace Domain.DTO.FrontEndDispatchCommunication;

public class CancelDeliveryResponse
{
    public string OrderId { get; set; }
    public bool IsCancelled { get; set; }
}
