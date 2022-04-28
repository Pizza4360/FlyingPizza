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
    private GeoLocation _homeLocation;


    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JsRuntime.InvokeVoidAsync("initGeocoder");
        }
    }
    protected override void OnInitialized()
    {
        _homeLocation = new GeoLocation{ Latitude = 39.74386695629378m, Longitude = -105.00610500179027m };
        _frontEndToDispatchGateway = new FrontEndToDispatchGateway();
        _frontEndToDatabaseGateway = new FrontEndToDatabaseGateway();
         converter = new ConvertAddressToGeoLocation();
         
    }

    public FrontEndToDispatchGateway GetGateway()
        => new FrontEndToDispatchGateway();


    public async Task<AddDroneResponse> AddDrone()
    {
        var newGuid = BaseEntity.GenerateNewGuid();


        var response = await _frontEndToDispatchGateway.AddDrone(new AddDroneRequest
        {
            DroneId = BaseEntity.GenerateNewId(),
            HomeLocation = _homeLocation,
            DroneUrl = "http://localhost:85",
            DispatchUrl = "http://localhost:83"
        });

        return response;
    }
    public async Task makeOrder()
    {

        var DeliveryLocation = await converter.CoordsFromAddress(DeliveryAddress);

        // var id = BaseEntity.GenerateNewId();

        var order = new Order
        {
            OrderId = BaseEntity.GenerateNewId(),
            TimeOrdered = DateTime.Now,
            CustomerName = CustomerName,
            DeliveryLocation = DeliveryLocation,
            DeliveryAddress = DeliveryAddress
        };

        await _frontEndToDatabaseGateway.EnqueueOrder(new CreateOrderRequest {
            Order = order
        });

        Console.Write("AHHHHHH~~~~~~~~~~");
        
        // Navigate to page to display users current order. 
        _navigationManager.NavigateTo("/userPage", false);
    }
}