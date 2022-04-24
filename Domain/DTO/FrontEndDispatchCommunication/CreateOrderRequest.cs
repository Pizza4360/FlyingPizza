using System;

namespace Domain.DTO.FrontEndDispatchCommunication;

public class CreateOrderRequest
{
    public string CustomerName {get;set;}
    public string CustomerAddress{get;set;}
    public DateTime TimeOrdered{get;set;}
    private GeoLocation _location;
    public GeoLocation DeliveryLocation{get;set;}
}
