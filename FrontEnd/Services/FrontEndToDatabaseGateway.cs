using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.DTO;
using Domain.DTO.FrontEndDispatchCommunication;
using Domain.Entities;
using Domain.GatewayDefinitions;

namespace FrontEnd.Services;

public class FrontEndToDatabaseGateway : BaseGateway<App>
{
    public FrontEndToDatabaseGateway(string dbUrl)
    {
        DbUrl = dbUrl;
    }

    private string DbUrl { get; }

    public async Task<List<DroneEntity>> GetFleet()
    {
        var endpoint = $"{DbUrl}/GetFleet";
        var s = $"SendMessageGet<List<DroneModel>>({endpoint})";
        var response = await SendMessageGet<List<DroneEntity>>(endpoint);
        Console.WriteLine($"{s} => {string.Join("\n", response)}");
        return response;
    }   

    public async Task<List<DeliveryEntity>> GetDeliveries()
    {
        var response = await SendMessageGet<List<DeliveryEntity>>(
            $"{DbUrl}/GetDeliveries"
        );
        return response;
    }
    public async Task<CreateDeliveryResponse> CreateDelivery(
        CreateDeliveryRequest request)
    {
        Console.Write($"before FrontEndToDatabaseGateway.SendMessagePost,"
                      + $" request = {request}");
        
        return await SendMessagePost
            <CreateDeliveryRequest, CreateDeliveryResponse>(
            $"{DbUrl}/CreateDelivery", request);
    }

    public async Task<CancelDeliveryResponse> CancelDelivery(
        CancelDeliveryRequest request) 
        => await SendMessagePost<CancelDeliveryRequest, CancelDeliveryResponse>(
        $"{DbUrl}/CancelDelivery", request);

    public async Task<DroneEntity> GetDrone(
        string id)
    {
        return await SendMessageGet<string, DroneEntity>(
            $"{DbUrl}/GetDrone", id);
    }

    public async Task<DeliveryEntity> GetDelivery(
        string id)
    {
        return await SendMessageGet<string, DeliveryEntity>(
            $"{DbUrl}/GetDelivery", id);
    }

    public async Task<string> GetApiKey()
    {
        return await SendMessageGet<string>($"{DbUrl}/GetApiKey");
    }

    public async Task<GeoLocation> GetHomeLocation()
    {
        Console.WriteLine($"FrontEndToDatabaseGateway.GetHomeLocation({DbUrl}"
                          + "/GetHomeLocation)");
        return await SendMessageGet<GeoLocation>($"{DbUrl}/GetHomeLocation");
    }

    public async Task AddDrone(string droneUrl)
    {
        await SendMessagePost($"{DbUrl}/AddDrone", 
            new BaseDto{Message=droneUrl});
    }

    
    public async Task<string> GetDispatchUrl()
    {
        Console.WriteLine($"FrontEndToDatabaseGateway.GetDispatchUrl({DbUrl}"
                          + "/GetDispatchIp)");
        return await SendMessageGet<string>($"{DbUrl}/GetDispatchIp");
    }
}