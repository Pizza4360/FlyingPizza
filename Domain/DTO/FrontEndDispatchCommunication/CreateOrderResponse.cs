namespace Domain.DTO.FrontEndDispatchCommunication;

public class CreateDeliveryResponse : BaseDto
{
    public string DeliveryId { get; set; }
    public bool IsCompletedSuccessfullly { get; set; }
}