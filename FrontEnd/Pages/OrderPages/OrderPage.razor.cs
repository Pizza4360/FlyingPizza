﻿using System;
using System.Threading.Tasks;
using Domain;
using Domain.DTO;
using Domain.DTO.FrontEndDispatchCommunication;
using Domain.Entities;
using FrontEnd.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FrontEnd.Pages.OrderPages;

partial class OrderPage : ComponentBase
{
    private string _customerName;

    private string _deliveryAddress;

    [Inject] public IJSRuntime JsRuntime { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender) await JsRuntime.InvokeVoidAsync("initGeocoder");
    }

    protected override void OnInitialized()
    {
    }

    private async Task<AddDroneResponse> AddDrone()
    {
        return await FrontEndToDispatchGateway.AddDrone(new AddDroneRequest
        {
            DroneId = BaseEntity.GenerateNewId(),
            BadgeNumber = Guid.NewGuid(),
            HomeLocation = new GeoLocation {Latitude = 39.74386695629378m, Longitude = -105.00610500179027m},
            DroneUrl = "http://localhost:85",
            DispatchUrl = "http://localhost:83"
        });
    }

    private async Task MakeOrder()
    {
        var deliveryLocation = await Converter.CoordsFromAddress(_deliveryAddress);

        var orderId = BaseEntity.GenerateNewId();

        await FrontEndToDatabaseGateway.CreateOrder(new CreateOrderRequest
        {
            OrderId = orderId,
            TimeOrdered = DateTime.Now,
            CustomerName = _customerName,
            DeliveryLocation = deliveryLocation,
            DeliveryAddress = _deliveryAddress,
            State = OrderState.Waiting
        });

        Console.Write("AHHHHHH~~~~~~~~~~");

        var dispatchResponse = FrontEndToDispatchGateway.EnqueueOrder(new EnqueueOrderRequest
        {
            OrderLocation = deliveryLocation,
            OrderId = orderId
        });

        Console.WriteLine(dispatchResponse);
        // Navigate to page to display users current order. 
        NavigationManager.NavigateTo("/userPage", false);
    }
}