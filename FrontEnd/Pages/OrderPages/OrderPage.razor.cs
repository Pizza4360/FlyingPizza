using System;
using System.Threading.Tasks;
using Domain.DTO;
using Domain.DTO.FrontEndDispatchCommunication;
using Domain.Entities;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FrontEnd.Pages.OrderPages;

partial class OrderPage : ComponentBase
{
    private string _deliveryAddress;
    private string _customerName;
    private string _droneUrl;

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
        return await FrontEndToDispatchGateway.AddDrone(_droneUrl);
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

        // Navigate to page to display users current order. 
        NavigationManager.NavigateTo("/tracking", false);
    }
}