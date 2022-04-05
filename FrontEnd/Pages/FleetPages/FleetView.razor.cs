using System;
using System.Threading.Tasks;
using Domain.Entities;
using Microsoft.AspNetCore.Components;

namespace FrontEnd.Pages.FleetPages
{
    partial class FleetView : ComponentBase
    {
        public DroneFields[] Fleet = null;
        public int size;
        public Boolean connection = false;
        protected override async Task OnInitializedAsync()
        {
            try
            {
                Fleet = await restPoint.Get<DroneFields[]>("http://localhost:8080/Fleet/?sort={badgeNumber:1}");
                size = Fleet.Length;
                connection = true;
            }
            catch { 
            }         
        }

        public async Task GoToDrone(DroneFields drone)
        {
            globalData.currDrone = drone;
            await dialogService.OpenAsync<DetailedDrone>("View Drone");           
        }
      
    }
}
