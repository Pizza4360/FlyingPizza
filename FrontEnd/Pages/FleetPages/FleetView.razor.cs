using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Domain.Entities;
using FrontEnd.Services;
using Microsoft.AspNetCore.Components;

namespace FrontEnd.Pages.FleetPages;

partial class FleetView : ComponentBase
{
    public DroneRecord[] Fleet = null;
    public int size;
    public Boolean connection = false;
    protected override async Task OnInitializedAsync()
    {
        try
        {
            Console.WriteLine("Hello, world");
            var response = new HttpClient().GetAsync(
                    "http://35.173.218.215:80/DatabaseAccess/GetFleet")
                .Result.Content.ReadAsStringAsync().Result;
            Console.WriteLine(response);
            Fleet = HttpMethods.Get<List<DroneRecord>>( "http://35.173.218.215:80/DatabaseAccess/GetFleet", true).Result.ToArray();
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