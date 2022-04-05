using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;
using Domain.Entities;


namespace FlyingPizza.Pages.FleetPages
{
    partial class FleetView : ComponentBase
    {
        public DroneRecord[] Fleet = null;
        public int size;
        public Boolean connection = false;
        protected override async Task OnInitializedAsync()
        {
            try
            {
                Fleet = await restPoint.Get<DroneRecord[]>("http://localhost:8080/Fleet/?sort={badgeNumber:1}");
                size = Fleet.Length;
                connection = true;
            }
            catch { 
            }         
        }

        public async Task GoToDrone(DroneRecord drone)
        {
            globalData.currDrone = drone;
            await dialogService.OpenAsync<DetailedDrone>("View Drone");           
        }
      
    }
}
