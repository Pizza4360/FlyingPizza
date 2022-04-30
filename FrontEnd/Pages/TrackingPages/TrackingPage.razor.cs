using System;
using System.Diagnostics;
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
    private const int RefreshInterval = 2000;
    private Stopwatch _stopwatch;
    private int _stopwatchInterval = 10000;
    private Timer _timer;
    public bool connection;
    public string dropDownLabel;
    public DroneRecord[] filteredDrones;
    public DroneRecord[] Fleet;

    [Inject] public FrontEndToDatabaseGateway _FrontEndToDatabaseGateway { get; set; }

    [Inject] public IJSRuntime JsRuntime { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            Console.WriteLine("_frontEndToDatabaseGateway == null ?" + _frontEndToDatabaseGateway == null);
            await JsRuntime.InvokeVoidAsync("initGoogleMap", new {Lat = 39.74386695629378, Lng = -105.00610500179027});
        }
    }

    protected override async Task OnInitializedAsync()
    {
        await DisplayDroneAsync("Delivering");
        await Temp();
    }

    protected Task Temp()
    {
        _timer = new Timer(MarkerUpdateCallback, null, 0, RefreshInterval);
        _stopwatch = new Stopwatch();
        _stopwatch.Start();
        return Task.CompletedTask;
    }

    protected async Task DisplayDroneAsync(string filter)
    {
        try
        {
            dropDownLabel = filter;
            Fleet = (await _FrontEndToDatabaseGateway.GetFleet()).ToArray();
            filteredDrones = Fleet.Where(record => record.State.ToString() == filter).ToArray();
            connection = true;
        }
        catch
        {
        }
    }

    private async void MarkerUpdateCallback(object _)
    {
        await UpdateDroneMarkers();
    }

    private async Task UpdateDroneMarkers()
    {
        var droneRecords = await _frontEndToDatabaseGateway.GetFleet();
        var markers = droneRecords.Select(x =>
            new JsMarker
            {
                lat = (double) x.CurrentLocation.Latitude,
                lng = (double) x.CurrentLocation.Longitude,
                title = x.Id,
                color = x.State.GetColor()
            }).ToDictionary(x => x.title, x => x);
        await JsRuntime.InvokeVoidAsync("updateAll", markers);
    }

    public string Color(DroneRecord drone)
    {
        return drone.State.GetColor();
    }

    public class JsMarker
    {
        public string title { get; set; }
        public double lat { get; set; }
        public double lng { get; set; }
        public string color { get; set; }
    }
}