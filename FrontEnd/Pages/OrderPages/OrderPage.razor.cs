using System;
using System.Runtime.InteropServices;
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
    [Inject]
    public IJSRuntime JsRuntime {get;set; }

    public string DeliveryAddress;
    public string CustomerName;
    public string DroneInput;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JsRuntime.InvokeVoidAsync("initGeocoder");
        }
    }
    protected override void OnInitialized()
    {
         Converter = new ConvertAddressToGeoLocation();
    }

    public async Task<AddDroneResponse> AddDrone() {
        return await FrontEndToDispatchGateway.AddDrone(new AddDroneRequest
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

        var DeliveryLocation = await Converter.CoordsFromAddress(DeliveryAddress);

        string OrderId = BaseEntity.GenerateNewId();

        await FrontEndToDatabaseGateway.CreateOrder(new CreateOrderRequest {
            OrderId = OrderId,
            TimeOrdered = DateTime.Now,
            CustomerName = CustomerName,
            DeliveryLocation = DeliveryLocation,
            DeliveryAddress = DeliveryAddress,
            DroneInput = DroneInput,
            State = OrderState.Waiting
        });

        Console.Write("AHHHHHH~~~~~~~~~~");

        var dispatchResponse = FrontEndToDispatchGateway.EnqueueOrder(new EnqueueOrderRequest
        {
            OrderLocation = DeliveryLocation,
            OrderId = OrderId,
        });
            
        Console.WriteLine(dispatchResponse);
        // Navigate to page to display users current order. 
        NavigationManager.NavigateTo("/userPage", false);
    }
}