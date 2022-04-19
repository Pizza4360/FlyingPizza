using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.Entities;
using FrontEnd.Services;
using Microsoft.AspNetCore.Components;
using Radzen;
using Radzen.Blazor;


namespace FrontEnd.Pages.TrackingPages;

partial class TrackingPage : ComponentBase
{
    public IEnumerable<DroneRecord> DroneRecords;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            DroneRecords = await HttpMethods.Get<List<DroneRecord>>(
                "http://localhost:5127/DatabaseAccess/GetFleet");
        }
        catch{}
    }
}
