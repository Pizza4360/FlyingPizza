using System;
using System.Collections.Generic;
using Domain.GatewayDefinitions;
using System.Threading.Tasks;
using Domain.DTO.FrontEndDispatchCommunication;
using Domain.Entities;

namespace FrontEnd.Services;

public class FrontEndToDatabaseGateway : BaseGateway<App>
{
    private string DbUrl{ get; } 
    public FrontEndToDatabaseGateway(/*string ipAddress*/)
    {
        DbUrl = "http://localhost:80/DatabaseAccess";
    }
    
    public async Task<List<DroneRecord>> GetFleet()
    {
        Console.WriteLine("Getting fleet...");
        Console.WriteLine($"SendMessageGet<List<DroneRecord>>({DbUrl}/GetFleet");
        var response = await SendMessageGet<List<DroneRecord>>($"{DbUrl}/GetFleet");
        Console.WriteLine("Got back" + string.Join("\n", response));
        return response;
    }

    /*   public async Task<CreateOrderResponse> CreateOrder(CreateOrderRequest request)
           => await SendMessagePost<CreateOrderRequest, CreateOrderResponse>($"{DispatchUrl}/EnqueueOrder", request);*/

    public async Task<CreateOrderResponse> CreateOrder(CreateOrderRequest request)
    {
        Console.Write($"before FrontEndToDatabaseGateway.SendMessagePost, request = {request}");
        return await SendMessagePost<CreateOrderRequest, CreateOrderResponse>($"{DbUrl}/CreateOrder", request);
    }

    public async Task<DroneRecord> GetDrone(string id)
        => await SendMessagePost<string, DroneRecord>($"{DbUrl}/GetDrone", id);
    
    public async Task<Order> GetOrder(string id)
        => await SendMessagePost<string, Order>($"{DbUrl}/GetOrder", id );
}
