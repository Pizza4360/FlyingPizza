using FlyingPizza.Drone;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlyingPizza.Pages.OrderPages
{
    partial class OrderPage : ComponentBase
    {
        Order custOrder;

        protected override async Task OnInitializedAsync()
        {
            // Make a new order for the customer to use when the page boots up
            custOrder = new Order();

            // send order to database empty
            var r = await restpoint.Post<Order>("http://localhost:8080/Orders", custOrder);
            custOrder.URL = r.Headers.Location.AbsoluteUri;
        }

        public async Task makeOrder()
        {
            // get time they ordered it
            custOrder.TimeOrdered = DateTime.Now.ToString();
            // upload final object to the server. 
            var r = await restpoint.Put<Order>(custOrder.URL,custOrder);
            // Navigate to page to display users current order. 
            // _navigationManager.NavigateTo("/userPage", false);
        }


    }
}
