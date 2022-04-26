using System;
using Domain.Entities;

namespace Domain.DTO.FrontEndDispatchCommunication;

public class CreateOrderRequest
{
    public Order Order{get;set;}
}
