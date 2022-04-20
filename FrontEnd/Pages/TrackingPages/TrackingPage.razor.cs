using System;
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
    private const double Tolerance = 0.0000000000001;
    private static readonly Dictionary<string, RadzenGoogleMapMarker> MarkerDict = new();
    private const int RefreshInterval = 2000;
    private Timer _timer;
    private IEnumerable<RadzenGoogleMapMarker> _markerData;
    private bool HasUpdatedMarkers { get; set; }
    private bool FirstUpdate { get; set; } = true;

    protected override bool ShouldRender() => FirstUpdate || HasUpdatedMarkers;

    protected override Task OnInitializedAsync()
    {
        _timer = new Timer(MarkerUpdateCallback, null, 0, RefreshInterval);
        return Task.CompletedTask;
    }
    
    private async void MarkerUpdateCallback(object _) => await UpdateDroneMarkers();

    private async Task UpdateDroneMarkers()
    {
        try
        {
            if (_markerData != null)
            {
                Console.WriteLine(MarkerDict.Values.First().Map.Zoom);
            }
            else
            {
                Console.WriteLine("_markerData in null");
            }
            var newMarkers = (await HttpMethods.Get<List<DroneRecord>>("http://localhost:5127/DatabaseAccess/GetFleet"))
                .Select((drone, i) => ToIdAndMarkerPair(i, drone))
                .ToDictionary(pair => pair.Key, pair => pair.Value);
            foreach (var (removalKey, _) in MarkerDict.Where(x => !newMarkers.ContainsKey(x.Key)))
            {
                MarkerDict[removalKey].Dispose();
                MarkerDict.Remove(removalKey);
            }

            HasUpdatedMarkers = false;
            foreach (var (droneId, droneMarker) in newMarkers)
            {
                if (!MarkerDict.ContainsKey(droneId))
                {
                    HasUpdatedMarkers = true;
                    MarkerDict[droneId] = droneMarker;
                }
                else
                {
                    var positionChanged = HasNewMarkerPosition(droneId, droneMarker);
                    if (!positionChanged) continue;
                    UpdateMarker(droneId, droneMarker);
                }
            }

      
            // Todo: update graphics without calling StateHasChanged to avoid resetting
            _markerData = MarkerDict.Values;
            /*if (FirstUpdate)
            {
                */
            await InvokeAsync(StateHasChanged);
                /*FirstUpdate = false;
            }*/
        }
        catch
        {
            // ignored
        }
    }

    private void UpdateMarker(string droneId, RadzenGoogleMapMarker droneMarker)
    {
        HasUpdatedMarkers = true;
        MarkerDict[droneId].Visible = true;
        MarkerDict[droneId].Position.Lat = droneMarker.Position.Lat;
        MarkerDict[droneId].Position.Lng = droneMarker.Position.Lng;
        MarkerDict[droneId].Visible = false;
    }

    private static KeyValuePair<string, RadzenGoogleMapMarker> ToIdAndMarkerPair(int i, DroneRecord drone) =>
        new(drone.Id, new RadzenGoogleMapMarker
        {

            Title = $"Drone #{i}",
            Label = $"Drone #{i}",
            Position = new GoogleMapPosition
            {
                Lat = (double) drone.CurrentLocation.Latitude,
                Lng = (double) drone.CurrentLocation.Longitude
            }
        });

    private static bool HasNewMarkerPosition(string droneId, RadzenGoogleMapMarker newMarker) 
        => Math.Abs(MarkerDict[droneId].Position.Lat - newMarker.Position.Lat) > Tolerance 
           || Math.Abs(MarkerDict[droneId].Position.Lng - newMarker.Position.Lng) > Tolerance;
}
