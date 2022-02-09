using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlyingPizza.Drone;


namespace FlyingPizza.Pages.FleetPages
{
    partial class FleetView : ComponentBase
    {

        public string[] block;
        public DroneModel[] Fleet = null;
        public int size;

        protected override async Task OnInitializedAsync()
        {
            Fleet = await restPoint.Get<DroneModel[]>("http://localhost:8080/Fleet/?sort={badgeNumber:1}");
            size = Fleet.Length;

        }
       /* protected override void OnInitialized()
        {
            Greeting = "Hello";
            block = new string[5]{ "drone1", "drone2", "drone3", "drone4", "drone5" };
        } */
    }
}
