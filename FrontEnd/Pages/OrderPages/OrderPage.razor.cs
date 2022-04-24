using System;
using System.Threading.Tasks;
using Domain.DTO.FrontEndDispatchCommunication;
using Domain.Entities;
using FrontEnd.Services;
using Microsoft.AspNetCore.Components;

namespace FrontEnd.Pages.OrderPages
{
    partial class OrderPage : ComponentBase
    {
        Order custOrder;

        // = new ("http://localhost:80"); 
        protected override void OnInitialized()
        {
            // Make a new order for the customer to use when the page boots up
            custOrder = new Order();
        
        }

        public FrontEndToDispatchGateway GetGateway()
            => new FrontEndToDispatchGateway
            {
                Url = "http://localhost:80"
            };

        public async Task makeOrder()
        {
            // get time they ordered it
            custOrder.TimeOrdered = DateTime.Now;


            // upload final object to the server. 
            var r = await HttpMethods.Post("http://localhost:5127/DatabaseAccess/AddOrder", custOrder);
            if (r.Headers.Location != null) custOrder.Id = r.Headers.Location.AbsoluteUri;
            var response = await HttpMethods.Put(custOrder.Id,custOrder);

            var dispatchResponse = _gateway.AddOrder(new AddOrderRequest
            {
                DeliveryLocation = custOrder.DeliveryLocation,
                OrderId = custOrder.Id
            });
            
            Console.WriteLine(dispatchResponse);
            // Navigate to page to display users current order. 
            _navigationManager.NavigateTo("/userPage", false);
        }
    }
}
