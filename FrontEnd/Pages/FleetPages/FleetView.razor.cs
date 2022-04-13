using System;
using System.Threading.Tasks;
using Domain.DTO;
using Domain.DTO.FrontEndDispatchCommunication;
using Domain.Entities;
using Microsoft.AspNetCore.Components;

namespace FrontEnd.Pages.FleetPages
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
            await dialogService.OpenAsync<DetailedDrone>("View SimDrone");           
        }
    }

    partial class AddDroneView : ComponentBase
    {
        public Guid BadgeNumber;
        public string DroneId;
        public string DroneIpAddress;
        public string DispatchIpAddress;
        public GeoLocation HomeLocation;
        public async Task<AddDroneResponse> AddDrone()
        {
            var request = new AddDroneRequest
            {
                BadgeNumber = Guid.NewGuid(),
                DispatchIp = DispatchIpAddress,
                DroneIp = DroneIpAddress,
                HomeLocation = HomeLocation,
                Id = DroneRecord.NewId()
            };
            //Todo: add a FrontEndToDispatchGateway so this works
            /*
            AddDroneResponse response = _dispatch.AddDrone(request);
            if (response.Success)
            {
                // restPoint.put_the_drone_record_in_the_database
            }
            return response;
            */
            throw new NotImplementedException();
        }  
    }
}
