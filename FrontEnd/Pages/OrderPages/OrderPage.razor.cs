﻿using System;
using System.Threading.Tasks;
using Domain;
using Domain.DTO;
using Domain.DTO.FrontEndDispatchCommunication;
using Domain.Entities;
using FrontEnd.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using MongoDB.Bson;
using Newtonsoft.Json.Linq;

namespace FrontEnd.Pages.OrderPages;

partial class OrderPage : ComponentBase
{
    [Inject]
    public IJSRuntime JsRuntime {get;set; }

    public string DeliveryAddress;
    public string CustomerName;
    public DroneRecord[] Fleet;
    public bool connection;
    protected override async void OnInitialized()
    {
        
        try
        {
            _frontEndToDatabaseGateway = new FrontEndToDatabaseGateway();
            _frontEndToDispatchGateway = new FrontEndToDispatchGateway();
            converter = new ConvertAddressToGeoLocation();
            Fleet = (await _frontEndToDatabaseGateway.GetFleet()).ToArray();
            connection = true;
        }
        catch
        {

        }

    }

    public FrontEndToDispatchGateway GetGateway()
        => new FrontEndToDispatchGateway();


    public async Task<AddDroneResponse> AddDrone() {
        return await _frontEndToDispatchGateway.AddDrone(new AddDroneRequest
        {
            DroneId = BaseEntity.GenerateNewId(),
            BadgeNumber = Guid.NewGuid(),
            HomeLocation = new GeoLocation{ Latitude = 39.74386695629378m, Longitude = -105.00610500179027m },
            DroneUrl = "http://localhost:85",
            DispatchUrl = "http://localhost:83"
        });
        
    }
    public async Task makeOrder()
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

        var dispatchResponse = _gateway.EnqueueOrder(new EnqueueOrderRequest
        {
            OrderLocation = DeliveryLocation,
            OrderId = OrderId,
        });
            
        Console.WriteLine(dispatchResponse);

    }
}