namespace Domain.DTO.FrontEndDispatchCommunication;

public class AddOrderRequest
    : BaseDto
{
    public string OrderId { get; set; }
    public GeoLocation DeliveryLocation { get; set; }
}