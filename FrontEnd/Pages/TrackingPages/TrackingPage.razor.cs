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
using Microsoft.JSInterop;


namespace FrontEnd.Pages.TrackingPages;

partial class TrackingPage : ComponentBase
{
    private const double Tolerance = 0.0000000000001;
    private static readonly Dictionary<string, GoogleMapPosition> MarkerDict = new();
    private static readonly Dictionary<string, int> indexToIdDict = new();
    private const int RefreshInterval = 2000;
    private Timer _timer;
    private IEnumerable<GoogleMapPosition> _markerData;
    [Inject]
    public IJSRuntime JsRuntime {get;set; }
    private bool HasUpdatedMarkers { get; set; }
    
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

    private async Task UpdateDroneMarkers()
    {
        // for(int i = 0; i < MarkerDict.Count; i++)
        // {
            await JsRuntime.InvokeVoidAsync("removeAllMarkers");
        // }
        var droneRecords = (await HttpMethods.Get<List<DroneRecord>>
             ("http://localhost:5127/DatabaseAccess/GetFleet")).Select((_, i) => (_, i));

         foreach(var (val, i) in droneRecords)
        {
            await JsRuntime.InvokeVoidAsync("addMarker", "drone" + i, val.CurrentLocation.Latitude, val.CurrentLocation.Longitude);
        }
        /*

        Console.WriteLine("num drones: " + newMarkers.Count + ", " + string.Join("\n,", newMarkers.Select(x => $"lat:{x.Value.Lat}, Lng:{x.Value.Lng}").ToList()) + "\n\n");
        foreach (var (removalKey, _) in GetRemovalDict(newMarkers))
        {
            await RemoveMarker(removalKey);
        }

        foreach (var (updatedKey, updatedMarker) in GetUpdateDict(newMarkers))
        {
            await UpdateMarker(updatedKey, updatedMarker);
        }

        foreach (var (addedKey, addedMarker) in GetAddedKeyDict(newMarkers))
        {
            await AddMarker(addedKey, addedMarker);
        }*/
    }


    private static IEnumerable<KeyValuePair<string, GoogleMapPosition>> 
        GetRemovalDict(Dictionary<string, GoogleMapPosition> newMarkers)
    => MarkerDict
           .Where(x => !newMarkers.ContainsKey(x.Key))
        .ToDictionary(pair => pair.Key, pair => pair.Value);

    private static IEnumerable<KeyValuePair<string, GoogleMapPosition>> 
        GetAddedKeyDict(Dictionary<string, GoogleMapPosition> newMarkers)
    => newMarkers.Where(x => !MarkerDict.ContainsKey(x.Key))
        .ToDictionary(pair => pair.Key, pair => pair.Value);

    private static IEnumerable<KeyValuePair<string, GoogleMapPosition>> 
        GetUpdateDict(Dictionary<string, GoogleMapPosition> newMarkers)
    => MarkerDict
      .Where(x => newMarkers.ContainsKey(x.Key) &&
                             newMarkers[x.Key] != MarkerDict[x.Key])
      .ToDictionary(pair => pair.Key, pair => pair.Value);


    private async Task RemoveMarker(string removalKey)
    {
        var index = indexToIdDict[removalKey];
        await JsRuntime.InvokeVoidAsync("removeMarker", index);
        indexToIdDict.Remove(removalKey);
        MarkerDict.Remove(removalKey);
    }

    private async Task AddMarker(string addedKey, GoogleMapPosition addedMarker)
    {
        indexToIdDict[addedKey] = indexToIdDict.Count;
        await JsRuntime.InvokeVoidAsync("addMarker", "drone" + indexToIdDict.Count, addedMarker.Lat, addedMarker.Lng);
        MarkerDict[addedKey] = addedMarker;
    }


    private async Task UpdateMarker(string droneId, GoogleMapPosition droneMarker)
    {
        var index = indexToIdDict[droneId];
        await JsRuntime.InvokeVoidAsync("updateMarker", index, droneMarker.Lat, droneMarker.Lng);
        MarkerDict[droneId].Lat = droneMarker.Lat;
        MarkerDict[droneId].Lng = droneMarker.Lng;
    }

    private static KeyValuePair<string, GoogleMapPosition> ToIdAndMarkerPair(int i, DroneRecord drone) =>
        new(drone.Id,
            new GoogleMapPosition
            {
                Lat = (double) drone.CurrentLocation.Latitude,
                Lng = (double) drone.CurrentLocation.Longitude
            }
      );
}
