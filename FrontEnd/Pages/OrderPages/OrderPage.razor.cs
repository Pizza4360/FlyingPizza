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

    public async Task<AddDroneResponse> AddDrone() {
        var response = await _frontEndToDispatchGateway.AddDrone(new AddDroneRequest
        {
            DroneId = BaseEntity.GenerateNewId(),
            BadgeNumber = Guid.NewGuid(),
            HomeLocation = new GeoLocation{ Latitude = 39.74386695629378m, Longitude = -105.00610500179027m },
            DroneUrl = "http://localhost:85",
            DispatchUrl = "http://localhost:83"
        });
        await GetFleet();
        return response;
    }

    public async Task MakeOrder()
    {

        var DeliveryLocation = await converter.CoordsFromAddress(DeliveryAddress);

        string OrderId = BaseEntity.GenerateNewId();

        await _frontEndToDatabaseGateway.CreateOrder(new CreateOrderRequest {
            OrderId = OrderId,
            TimeOrdered = DateTime.Now,
            CustomerName = CustomerName,
            DeliveryLocation = DeliveryLocation,
            DeliveryAddress = DeliveryAddress
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