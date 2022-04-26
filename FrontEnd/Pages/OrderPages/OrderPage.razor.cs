using System;
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

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JsRuntime.InvokeVoidAsync("initGeocoder");
        }
    }
    protected override void OnInitialized()
    {
        _frontEndToDispatchGateway = new FrontEndToDispatchGateway();
        _frontEndToDatabaseGateway = new FrontEndToDatabaseGateway();
         converter = new ConvertAddressToGeoLocation();
    }

    public FrontEndToDispatchGateway GetGateway()
        => new FrontEndToDispatchGateway();


    public async Task<AddDroneResponse> AddDrone() {
        return await _frontEndToDispatchGateway.AddDrone(new AddDroneRequest
        {
            DroneId = ObjectId.GenerateNewId(),
            BadgeNumber = Guid.NewGuid(),
            HomeLocation = new GeoLocation{ Latitude = 39.74386695629378m, Longitude = -105.00610500179027m },
            DroneUrl = "http://localhost:85",
            DispatchUrl = "http://localhost:83"
        });
        
    }
    public async Task makeOrder()
    {

        var DeliveryLocation = await converter.CoordsFromAddress(DeliveryAddress);

        var id = BaseEntity.GenerateNewId();

        var order = new Order
        {
            OrderId = id,
            TimeOrdered = DateTime.Now,
            CustomerName = CustomerName,
            DeliveryLocation = DeliveryLocation,
            DeliveryAddress = DeliveryAddress
        };

        await _frontEndToDatabaseGateway.CreateOrder(new CreateOrderRequest {
            Order = order
        });

        Console.Write("AHHHHHH~~~~~~~~~~");

        var dispatchResponse = _gateway.EnqueueOrder(new EnqueueOrderRequest
        {
            Order = order,
        });
            
        Console.WriteLine(dispatchResponse);
        // Navigate to page to display users current order. 
        _navigationManager.NavigateTo("/userPage", false);
    }
}