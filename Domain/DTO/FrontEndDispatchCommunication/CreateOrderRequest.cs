﻿using System;

namespace Domain.DTO.FrontEndDispatchCommunication;

public class CreateOrderRequest
{
    private GeoLocation _location;

    public string OrderId { get; set; }
    public string CustomerName { get; set; }
    public string DeliveryAddress { get; set; }
    public DateTime TimeOrdered { get; set; }
    public GeoLocation DeliveryLocation { get; set; }
    public OrderState State { get; set; }
}