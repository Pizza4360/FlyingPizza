using System;
using System.Threading.Tasks;
using Domain.DTO;
using Domain.DTO.FrontEndDispatchCommunication;
using Domain.Entities;
using FrontEnd.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FrontEnd.Pages.OrderPages;

partial class OrderPage : ComponentBase
{
    public string DeliveryAddress;
    public string CustomerName;
    public string DroneInput;

        }
    }

    public async Task GetOrders()
    {
        Orders = (await _frontEndToDatabaseGateway.GetOrder()).ToArray();
    }
    public async Task GetFleet()
    {
        Fleet = (await _frontEndToDatabaseGateway.GetFleet()).ToArray();
    }

    private async Task<AddDroneResponse> AddDrone()
    {

        return await FrontEndToDispatchGateway.AddDrone(DroneInput);
    }

    public async Task MakeOrder()
    {
        var deliveryLocation = await Converter.CoordsFromAddress(DeliveryAddress);

        var orderId = BaseEntity.GenerateNewId();
        
        await FrontEndToDatabaseGateway.CreateOrder(new CreateOrderRequest
        {
            OrderId = orderId,
            TimeOrdered = DateTime.Now,
            CustomerName = CustomerName,
            DeliveryLocation = deliveryLocation,
            DeliveryAddress = DeliveryAddress,
            DroneInput = DroneInput,
            State = OrderState.Waiting
        });

        await GetOrders();

        var dispatchResponse = _gateway.EnqueueOrder(new EnqueueOrderRequest
        {
            OrderLocation = DeliveryLocation,
            OrderId = OrderId,
        });
            
        Console.WriteLine(dispatchResponse);

    }

    public async Task CancelOrder()
    {
        if (orderToCancel == null)
        {
            orderToCancel = selectedOrder.Id;
        }
        defaultText = orderToCancel;

        var dispatchResponse = await _frontEndToDatabaseGateway.CancelOrder(new CancelDeliveryRequest
        {
           OrderId = orderToCancel
        });
        await GetOrders();
        orderToCancel = null;
        defaultText = "";
        Console.WriteLine(dispatchResponse);
    }

    public void OnInfoClose(){
        visibility = "hidden";
        defaultText = "";
        selectedOrder = null;
    }

    public void DisplaySelected(Order selected)
    {
        selectedOrder = selected;
        defaultText = selectedOrder.Id;
        visibility = "visible";
    }

    public string Color(DroneRecord drone)
    {
        return drone.State.GetColor();
    }
}