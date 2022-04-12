using System;
using System.Threading.Tasks;
using Domain.DTO;
using Domain.DTO.Shared;
using Domain.Entities;
using Domain.InterfaceImplementations.Gateways;
using Microsoft.AspNetCore.Components;

namespace FrontEnd.Pages.OrderPages
{
    partial class OrderPage : ComponentBase
    {
        Order custOrder;

        private static FrontEndToDispatchGateway _gateway
            = new ();

        protected override void OnInitialized()
        {
            // Make a new order for the customer to use when the page boots up
            custOrder = new Order();

        }

        public async Task makeOrder()
        {
            // get time they ordered it
            custOrder.TimeOrdered = DateTime.Now;

            // upload final object to the server. 
            var r = await restpoint.Post<Order>("http://localhost:8080/Orders", custOrder);
            custOrder.URL = r.Headers.Location.AbsoluteUri;
            var response = await restpoint.Put<Order>(custOrder.URL,custOrder);
            _gateway.SendMessage(
                "AddOrder"
                , new AddOrderRequest(
                    custOrder.Id
                    , custOrder.DeliveryLocation));
            // Navigate to page to display users current order. 
            // _navigationManager.NavigateTo("/userPage", false);
        }
    }

    public class AddOrderRequest
        : BaseDTO
    {
        private string OrderId { get; set; }
        private GeoLocation DeliveryLocation { get; set; }
        public AddOrderRequest(string custOrderId, GeoLocation custOrderDeliveryLocation)
        {
            
        }
    }
}
