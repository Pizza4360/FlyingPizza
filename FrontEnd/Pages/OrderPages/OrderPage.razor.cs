using System;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.AspNetCore.Components;

namespace FrontEnd.Pages.OrderPages
{
    partial class OrderPage : ComponentBase
    {
        Order custOrder;

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
            r = await restpoint.Put<Order>(custOrder.URL,custOrder);
            // Navigate to page to display users current order. 
            // _navigationManager.NavigateTo("/userPage", false);
        }
    }
}
