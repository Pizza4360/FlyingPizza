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

        protected override void OnInitialized()
        {
            // Make a new order for the customer to use when the page boots up
            custOrder = new Order();

        }

        public async Task makeOrder()
        {
            // get time they ordered it
            custOrder.TimeOrdered = DateTime.Now.ToString();

            // upload final object to the server. 
            var r = await restpoint.Post<Order>("http://localhost:8080/Orders", custOrder);
            var r2 = await _dispatcherGateway.PostOrder(custOrder);
            custOrder.URL = r.Headers.Location.AbsoluteUri;
            r = await restpoint.Put<Order>(custOrder.URL,custOrder);
            // Navigate to page to display users current order. 
            // _navigationManager.NavigateTo("/userPage", false);
        }

        public Point[] GetRoute(Point location, Point delivery)
        {
            if (Math.Abs(location.Lat - delivery.Lat) < Point.Tolerance 
                && Math.Abs(location.Long - delivery.Long) < Point.Tolerance)
            {
                throw new ArgumentException(
                    "Destination cannot be the same as the Delivery station!");
            }

            // Longitude distance to get to destination
            var xDistance = location.Lat - delivery.Lat;

            // Latitude distance to get to destination
            var yDistance = location.Long - delivery.Long;

            // # of steps should be the absolute value of the hypotenuse,
            // rounded up to the nearest integer
            var stepsCount = Math.Abs((int)Math.Ceiling(Math.Sqrt(
                xDistance * xDistance + yDistance * yDistance)));
            
            // The incremental change in latitude & longitude for each discrete
            // Point
            var xStep = Math.Abs(delivery.Lat - location.Lat) / stepsCount;
            var yStep = Math.Abs(delivery.Long - location.Long) / stepsCount;

            // The multiplier to ensure the direction of StepSize
            // increases for Destination X and Y > Home X and Y
            // decreases for Destination X and Y < Home X and Y
            var xDirection = delivery.Lat > location.Lat ? 1 : -1;
            var yDirection = delivery.Long > location.Long ? 1 : -1;
            
            Point[] route = new Point[stepsCount];

            for (var i = 0; i < stepsCount - 1; i++)
            {
                route[i] = new Point((i + 1) * xStep * xDirection,
                    (i + 1) * yStep * yDirection);
            }

            route[stepsCount - 1] = delivery;
            return route;
        }
    }
}
