using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;
using FrontEnd.Services;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;


namespace FrontEnd.Pages.TrackingPages;

partial class TrackingPage : ComponentBase
{
    public static IEnumerable<DroneRecord> DroneRecords;
    private const int RefreshInterval = 2000;
    private Timer _timer;
    public IEnumerable<RadzenGoogleMapMarker> MarkerData;

    protected override Task OnInitializedAsync()
    {
        _timer = new Timer(
            Callback,
            null, 
            0,
            RefreshInterval);
        return Task.CompletedTask;
    }

    private async void Callback(object _) => await UpdateDroneMarkers();

    private async Task UpdateDroneMarkers()
    {
        try
        {
            MarkerData = (await HttpMethods.Get<List<DroneRecord>>("http://localhost:5127/DatabaseAccess/GetFleet"))
                .Select(
                    (drone, i) => new RadzenGoogleMapMarker
                    {
                      // Id=@(drone.BadgeNumber) 
                        Title="Drone #" + i,
                        Label="Drone #" + i++,
                        Position=new GoogleMapPosition {
                            Lat = (double) drone.CurrentLocation.Latitude, 
                            Lng = (double) drone.CurrentLocation.Longitude 
                        }
                    });
            
            // await InvokeAsync(StateHasChanged);
        }
        catch{}
    }

    public class OurMap : RadzenGoogleMap
    {
        
    }
}
