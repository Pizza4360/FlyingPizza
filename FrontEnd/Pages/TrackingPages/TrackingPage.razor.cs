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
    
    protected override Task OnInitializedAsync()
    {
        _timer = new Timer(MarkerUpdateCallback, null, 0, RefreshInterval);
        return Task.CompletedTask;
    }
    
    private async void MarkerUpdateCallback(object _) => await UpdateDroneMarkers();

    private async Task UpdateDroneMarkers()
    {
        var newMarkers = (await HttpMethods.Get<List<DroneRecord>>
                             ("http://localhost:5127/DatabaseAccess/GetFleet"))
            .Select((drone, i) => ToIdAndMarkerPair(i, drone))
            .ToDictionary(pair => pair.Key, pair => pair.Value);

   
        foreach (var (removalKey, _) in GetRemovedKeyValuePairs(newMarkers))
        {
            DeleteMarker(removalKey);
        }

        foreach (var (updatedKey, updatedMarker) in GetUpdatedKeyValuePairs(newMarkers))
        {
            UpdateMarker(updatedKey, updatedMarker);
        }

        foreach (var (addedKey, addedMarker) in GetAddedKeyValuePairs(newMarkers))
        {
            AddMarker(addedKey, addedMarker);
        }

        _markerData = MarkerDict.Values;
        if (HasUpdatedMarkers)
        {
            await InvokeAsync(StateHasChanged);
        }
    }


    private static IEnumerable<KeyValuePair<string, RadzenGoogleMapMarker>> 
        GetRemovedKeyValuePairs(Dictionary<string, RadzenGoogleMapMarker> newMarkers)
    => MarkerDict
           .Where(x => !newMarkers.ContainsKey(x.Key));


    private static IEnumerable<KeyValuePair<string, RadzenGoogleMapMarker>> 
        GetAddedKeyValuePairs(Dictionary<string, RadzenGoogleMapMarker> newMarkers)
    => newMarkers.Where(x => !MarkerDict.ContainsKey(x.Key));
    


    private static IEnumerable<KeyValuePair<string, RadzenGoogleMapMarker>> 
        GetUpdatedKeyValuePairs(Dictionary<string, RadzenGoogleMapMarker> newMarkers)
    => MarkerDict
           .Where(x => newMarkers.ContainsKey(x.Key) &&
                       newMarkers[x.Key].Position != MarkerDict[x.Key].Position);


    private void DeleteMarker(string removalKey)
    {
        HasUpdatedMarkers = true;
        MarkerDict[removalKey].Dispose();
        MarkerDict.Remove(removalKey);
    }


    private void AddMarker(string addedKey, RadzenGoogleMapMarker addedMarker)
    {
        HasUpdatedMarkers = true;
        MarkerDict[addedKey] = addedMarker;
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
            },
        });

    private static bool HasNewMarkerPosition(string droneId, RadzenGoogleMapMarker newMarker) 
        => MarkerDict[droneId].Position.Lat == newMarker.Position.Lat || 
           MarkerDict[droneId].Position.Lng == newMarker.Position.Lng;
}
