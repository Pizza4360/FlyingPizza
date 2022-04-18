namespace Domain.DTO.FrontEndDispatchCommunication;

public class CancelDeliveryResponse : BaseDto
{
    public string OrderId { get; set; }
    public bool IsCancelled { get; set; }
}
