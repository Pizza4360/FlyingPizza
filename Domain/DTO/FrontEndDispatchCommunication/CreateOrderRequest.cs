using System;

namespace Domain.DTO.FrontEndDispatchCommunication;

public class CreateDeliveryRequest : BaseDto
{
    private GeoLocation _location;

    public string DeliveryId { get; set; }
    public string CustomerName {get;set;}
    public string DeliveryAddress{get;set;}
    public string DroneId { get;set; }
    public DateTime TimeDeliveryed{get;set;}
    // private GeoLocation _location;
    public GeoLocation DeliveryLocation{get;set;}
    public Deliveriestatus Status { get; set; }
}