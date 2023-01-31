using System;
using System.Threading.Tasks;
using Domain.DTO;
using Domain.DTO.FrontEndDispatchCommunication;
using Domain.Entities;
using FrontEnd.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace FrontEnd.Pages.DeliveryPages;

partial class DeliveryPage : ComponentBase
{
    public string DeliveryAddress;
    public string CustomerName;
    public string DroneUrl;
    public DeliveryEntity[] Deliveries;

    public bool connection;

    public DeliveryEntity SelectedDeliveryEntity;

    public string visibility = "hidden";

    public string deliveryToCancel;

    public string defaultText ="";
    public DroneEntity[] Fleet;
    
    protected override async Task OnInitializedAsync()
    {
        try
        {          
            await GetDeliveries();
            await GetFleet();
            connection = true;
        }
        catch
        {

        }
    } 

    public async Task GetDeliveries()
    {
        Deliveries = (await DatabaseGateway.GetDeliveries()).ToArray();
    }
    public async Task GetFleet()
    {
        Fleet = (await DatabaseGateway.GetFleet()).ToArray();
    }

    private async Task AddDrone()
    {
        await DatabaseGateway.AddDrone(DroneUrl);
    }

    public async Task MakeDelivery()
    {
        var deliveryId = BaseEntity.GenerateNewId();
        await DatabaseGateway.CreateDelivery(new CreateDeliveryRequest
        {
            DeliveryId = deliveryId,
            TimeDeliveryed = DateTime.Now,
            CustomerName = CustomerName,
            DeliveryLocation = null,
            DeliveryAddress = DeliveryAddress,
            DroneId = DroneUrl,
            Status = Deliveriestatus.Waiting
        });
        await GetDeliveries(); 
    }

    public async Task CancelDelivery()
    {
        if (!string.IsNullOrEmpty(deliveryToCancel))
        {
            deliveryToCancel = SelectedDeliveryEntity.Id;
        }
        defaultText = deliveryToCancel;
        await GetDeliveries();
        deliveryToCancel = null;
        defaultText = "";
    }

    public void OnInfoClose(){
        visibility = "hidden";
        defaultText = "";
        SelectedDeliveryEntity = null;
    }

    public void DisplaySelected(DeliveryEntity selected)
    {
        SelectedDeliveryEntity = selected;
        defaultText = SelectedDeliveryEntity.Id;
        visibility = "visible";
    }

    public string Color(DroneEntity drone)
    {
        return drone.LatestStatus.GetColor();
    }
    
}