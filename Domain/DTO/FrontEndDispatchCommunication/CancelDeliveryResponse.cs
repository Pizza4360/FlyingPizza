namespace Domain.DTO.FrontEndDispatchCommunication;

public class CancelDeliveryResponse : BaseDTO
{
    public string OrderId { get; set; }
    public bool IsCancelled { get; set; }
}
