using Domain.DTO;
using Domain.Entities;
using FrontEnd.Services;
using Microsoft.AspNetCore.Components;

namespace FrontEnd.Pages.FleetPages;

partial class FleetView : ComponentBase
{
    public DroneRecord[] Fleet;
    public int size;
    public Boolean connection;
    protected override async Task OnInitializedAsync()
    {
        try
        {
            var droneRecords = (await HttpMethods.Get<List<DroneRecord>>("http://localhost:5127/DatabaseAccess/GetFleet"));
            Fleet = droneRecords.ToArray();
            var readyDrones = droneRecords.Where(record => record.State == DroneState.Ready);
            size = Fleet.Length;
            connection = true;
        }
        catch {}
    }

    public async Task GoToDrone(DroneRecord drone)
    {
        globalData.currDrone = drone;
        await dialogService.OpenAsync<DetailedDrone>("View SimDrone");           
    }
}