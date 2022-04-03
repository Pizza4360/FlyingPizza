using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;
using FlyingPizza.Domain.Entities;


namespace FlyingPizza.Pages.FleetPages
{
    partial class FleetView : ComponentBase
    {
        public DroneModel[] Fleet = null;
        public int size;
        public Boolean connection = false;
        protected override async Task OnInitializedAsync()
        {
            try
            {
                Fleet = await restPoint.Get<DroneModel[]>("http://localhost:8080/Fleet/?sort={badgeNumber:1}");
                size = Fleet.Length;
                connection = true;
            }
            catch { 
            }         
        }

        public async Task GoToDrone(DroneModel drone)
        {
            globalData.currDrone = drone;
            await dialogService.OpenAsync<DetailedDrone>("View Drone");           
        }
      
    }
}
