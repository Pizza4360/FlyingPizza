using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.DTO;
using Domain.Entities;
using FrontEnd.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FrontEnd.Pages.FleetPages;

partial class FleetView : ComponentBase
{
    public DroneRecord[] Fleet;
    public int size;
    public Boolean connection;
    public string color;

    [Inject]
    public IJSRuntime JsRuntime { get; set; }
    protected override async Task OnInitializedAsync()
    {
        try
        {
            Fleet = (await HttpMethods.Get<List<DroneRecord>>("http://localhost:5127/DatabaseAccess/GetFleet")).ToArray();

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

    public string Color(DroneRecord drone)
    {
        return drone.State.GetColor();
    }

}