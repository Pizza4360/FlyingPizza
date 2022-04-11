using System;
using System.Net.Http;
using System.Threading.Tasks;
using Domain.DTO.DroneDispatchCommunication;
using Domain.DTO.FrontEndDispatchCommunication;
using Domain.DTO.Shared;
using Domain.InterfaceDefinitions.Gateways;

namespace Domain.InterfaceImplementations.Gateways;

public class FrontEndToDispatchGateway : BaseGateway
{
    // Step 4, DroneToDispatchGateway takes in initial info
    // to create a GeoLocation and then POST its first status update 
    public Task<HttpResponseMessage> AddDrone(
    int latitude,
    int longitude,
    string ready) 
        => SendMessage("PostInitialStatus", 
            new DroneStatusUpdateRequest 
            {
                Location = new GeoLocation
                {
                    Latitude = latitude,
                    Longitude = longitude
                },
                State = ready
            });
    
    public void RemoveDrone(HttpMessageHandler handler)
    {
        // Added for mocking reasons, no way around it
        HttpClient = new HttpClient(handler);
    }

    public async Task<Task<HttpResponseMessage>> SendDelivery(SendDeliveryRequest dto)
        => SendMessage("SendDelivery", dto);
    
    public async Task<Task<HttpResponseMessage>> CancelDeliveryRequest(string id)
        => SendMessage("CancelDeliveryRequest",
            new CancelDeliveryRequest
            {
                OrderId = id
            });

    /// <summary>
    /// This method gets called when a drone updates its status.
    /// </summary>
    /// <param name="state"></param>
    /// <returns></returns>
    public Task<Task<HttpResponseMessage>> PatchDroneStatus(DroneStatusUpdateRequest state)
        => Task.FromResult(SendMessage(Url, state));
}