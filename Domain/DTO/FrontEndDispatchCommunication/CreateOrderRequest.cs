using System;

namespace Domain.DTO.FrontEndDispatchCommunication;

public class CreateOrderRequest
{
    private GeoLocation _location;

    public string OrderId { get; set; }
<<<<<<< HEAD
    public string CustomerName {get;set;}
    public string DeliveryAddress{get;set;}
    public string DroneInput { get;set;}
    public DateTime TimeOrdered{get;set;}
    private GeoLocation _location;
    public GeoLocation DeliveryLocation{get;set;}
=======
    public string CustomerName { get; set; }
    public string DeliveryAddress { get; set; }
    public DateTime TimeOrdered { get; set; }
    public GeoLocation DeliveryLocation { get; set; }
>>>>>>> 6d20d48abdbe4b6f1e4fc2718815dc3e8a9aa048
    public OrderState State { get; set; }
}