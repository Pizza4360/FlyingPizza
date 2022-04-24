using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.DTO;
using Domain.Entities;
using FrontEnd.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FrontEnd.Pages.TrackingPages;

partial class TrackingPage : ComponentBase
{
    private const int RefreshInterval = 10;
    private Timer _timer;
    
    [Inject]
    public IJSRuntime JsRuntime {get;set; }
    
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JsRuntime.InvokeVoidAsync("initGoogleMap", new { Lat = 39.74386695629378, Lng = -105.00610500179027 });
        }
    }
    
    protected override Task OnInitializedAsync()
    {
        _timer = new Timer(MarkerUpdateCallback, null, 0, RefreshInterval);
        return Task.CompletedTask;
    }
    
    private async void MarkerUpdateCallback(object _) => await UpdateDroneMarkers();
    public class JsMarker
    {
        public string title{get;set;}
        public double lat{get;set;}
        public double lng{get;set;}
        public string color{get; set;}
    }

    private async Task UpdateDroneMarkers()
    {
        
        var markers = (await HttpMethods.Get<List<DroneRecord>>
            ("http://localhost:5127/DatabaseAccess/GetFleet")).Select(x => 
            new JsMarker{
                lat = (double)x.CurrentLocation.Latitude,
                lng = (double)x.CurrentLocation.Longitude,
                title = x.DroneId,
                color = x.State.GetColor()
            }).ToDictionary(x => x.title, x => x);
        await JsRuntime.InvokeVoidAsync("updateAll", markers);
    }
}
