using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.Entities;
using FrontEnd.Services;
using Microsoft.AspNetCore.Components;
using Radzen;


namespace FrontEnd.Pages.TrackingPages;

partial class TrackingPage : ComponentBase
{
    public static IEnumerable<DroneRecord> DroneRecords;
    private const int RefreshInterval = 2000;
    private Timer _timer;
    public GoogleMapPosition markers;

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
            DroneRecords = await HttpMethods.Get<List<DroneRecord>>("http://localhost:5127/DatabaseAccess/GetFleet");
            await InvokeAsync(StateHasChanged);
        }
        catch{}
    }
}
