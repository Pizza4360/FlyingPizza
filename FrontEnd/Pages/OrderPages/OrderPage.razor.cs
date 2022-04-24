using System;
using System.Threading.Tasks;
using Domain;
using Domain.DTO;
using Domain.DTO.FrontEndDispatchCommunication;
using Domain.Entities;
using FrontEnd.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Newtonsoft.Json.Linq;

namespace FrontEnd.Pages.OrderPages;

partial class OrderPage : ComponentBase
{
    [Inject]
    public IJSRuntime JsRuntime {get;set; }
    Order custOrder;


    // = new ("http://localhost:80"); 
    protected override void OnInitialized()
    {
        // Make a new order for the customer to use when the page boots up
        custOrder = new Order();
            
    }

    public FrontEndToDispatchGateway GetGateway()
        => new FrontEndToDispatchGateway();

        
    public async Task<GeoLocation> GetGeoLocationFromAddress(string address)
    {
        var js = JObject.Parse(await JsRuntime.InvokeAsync<string>("codeAddress", address));
        var lng = decimal.Parse(Convert.ToString(js["results"]?[0]?["geometry"]?["location"]?["lng"]) ?? string.Empty);
        var lat = decimal.Parse(Convert.ToString(js["results"]?[0]?["geometry"]?["location"]?["lat"]) ?? string.Empty);
        return new GeoLocation
        {
            Latitude = lat,
            Longitude = lng 
        };
    }

    public async Task makeOrder()
    {
        // get time they ordered it
        custOrder.TimeOrdered = DateTime.Now;


        // upload final object to the server. 
        var r = await HttpMethods.Post("http://localhost:5127/DatabaseAccess/EnqueueOrder", custOrder);
        if (r.Headers.Location != null) custOrder.Id = r.Headers.Location.AbsoluteUri;
        var response = await HttpMethods.Put(custOrder.Id,custOrder);

        var dispatchResponse = _gateway.EnqueueOrder(new EnqueueOrderRequest
        {
            OrderLocation = custOrder.DeliveryLocation,
            OrderId = custOrder.Id
        });
            
        Console.WriteLine(dispatchResponse);
        // Navigate to page to display users current order. 
        _navigationManager.NavigateTo("/userPage", false);
    }
}