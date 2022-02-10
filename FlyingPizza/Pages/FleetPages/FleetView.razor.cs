using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FlyingPizza.Drone;
using FlyingPizza.Pages.FleetPages;
using Radzen;



namespace FlyingPizza.Pages.FleetPages
{
    partial class FleetView : ComponentBase
    {

        public string[] block;
        public DroneModel[] Fleet = null;
        public int size;
        public string address;

        protected override async Task OnInitializedAsync()
        {
            Fleet = await restPoint.Get<DroneModel[]>("http://localhost:8080/Fleet/?sort={badgeNumber:1}");
            size = Fleet.Length;
            address = "890 Auraria Pkwy";

        }

        public async Task<Boolean> GoToDrone(DroneModel drone)
        {
            globalData.currDrone = drone;
            var r = await dialogService.OpenAsync<DetailedDrone>("View Drone");
            return r;
        }
      
    }
}
