using System;
using System.Threading.Tasks;
using Domain.DTO;
using Domain.DTO.FrontEndDispatchCommunication;
using Domain.DTO.Shared;
using Domain.Entities;
using Domain.InterfaceImplementations.Gateways;
using Microsoft.AspNetCore.Components;

namespace FrontEnd.Pages.OrderPages
{
    partial class OrderPage : ComponentBase
    {
        Order custOrder;

        private static FrontEndToDispatchGateway _gateway = new ("http://localhost:80");

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
            Console.WriteLine("made it here");
            var dispatchResponse = await _gateway.AddDrone(
                new AddOrderRequest()
                {
                    OrderId = custOrder.Id
                    , DeliveryLocation = custOrder.DeliveryLocation
                });
            // Navigate to page to display users current order. 
            // _navigationManager.NavigateTo("/userPage", false);
        }
    }

    
}
