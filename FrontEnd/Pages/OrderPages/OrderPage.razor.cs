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

        return await FrontEndToDispatchGateway.AddDrone(DroneInput);
    }

    private async Task MakeOrder()
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
        
        //var dispatchResponse = FrontEndToDispatchGateway.EnqueueOrder(new EnqueueOrderRequest
        //{
        //    OrderLocation = deliveryLocation,
        //    OrderId = orderId
        //});
        
        // Navigate to page to display users current order. 
        // NavigationManager.NavigateTo("/tracking", false);
    }
}