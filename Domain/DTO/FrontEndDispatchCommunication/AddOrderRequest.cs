namespace Domain.DTO.FrontEndDispatchCommunication;

public class AddOrderRequest
    : BaseDTO
{
    public string OrderId { get; set; }
    public GeoLocation DeliveryLocation { get; set; }
}