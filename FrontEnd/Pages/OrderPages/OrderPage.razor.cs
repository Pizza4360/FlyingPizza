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
        _frontEndToDatabaseGateway = new FrontEndToDatabaseGateway();
         converter = new ConvertAddressToGeoLocation();
    }

    public FrontEndToDispatchGateway GetGateway()
        => new FrontEndToDispatchGateway();

        

    public async Task makeOrder()
    {

        var DeliveryLocation = await converter.CoordsFromAddress(DeliveryAddress);

        string OrderId = BaseEntity.GenerateNewId();

        await _frontEndToDatabaseGateway.CreateOrder(new CreateOrderRequest {
            OrderId = OrderId,
            TimeOrdered = DateTime.Now,
            CustomerName = CustomerName,
            DeliveryLocation = DeliveryLocation,
            CustomerAddress = DeliveryAddress
        });

        Console.Write("AHHHHHH~~~~~~~~~~");

        var dispatchResponse = _gateway.EnqueueOrder(new EnqueueOrderRequest
        {
            OrderLocation = DeliveryLocation,
            OrderId = OrderId,
        });
            
        Console.WriteLine(dispatchResponse);
        // Navigate to page to display users current order. 
        _navigationManager.NavigateTo("/userPage", false);
    }
}