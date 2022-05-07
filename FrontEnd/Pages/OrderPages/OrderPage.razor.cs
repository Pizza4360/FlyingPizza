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

    private async Task AddDrone()
    {
        await DatabaseGateway.AddDrone(DroneInput);
    }

    private async Task MakeOrder()
    {
        var orderId = BaseEntity.GenerateNewId();
        await DatabaseGateway.CreateOrder(new CreateOrderRequest
        {
            OrderId = orderId,
            TimeOrdered = DateTime.Now,
            CustomerName = CustomerName,
            DeliveryLocation = null,
            DeliveryAddress = DeliveryAddress,
            DroneId = "",
            State = OrderState.Waiting
        });
    }
}