using System.Diagnostics;
using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;
using Domain.DTO.FrontEndDispatchCommunication;
using Domain.Entities;
using Domain.RepositoryDefinitions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace Dispatch.Controllers;

[ApiController]
[Route("[controller]")]
public class DispatchController : ControllerBase
{
    // private readonly ILogger<DispatchController> _logger;
    private const int RefreshInterval = 10000;
    private readonly GeoLocation _homeLocation;
    private readonly ICompositeRepository _repository;
    private readonly DispatchToSimDroneGateway _dispatchToSimDroneGateway;
    private readonly Stopwatch _stopwatch;
    
    
    private Timer _timer;
    private async void IssueAssignments(object _) => await TimerCheck();
    
    private async Task TimerCheck()
    {
        if(_stopwatch.ElapsedMilliseconds > RefreshInterval)
        {
            _stopwatch.Stop();
            _stopwatch.Reset();
            AssignEnqueuedDeliveries();
            _stopwatch.Start();
        }
    }
    public DispatchController(ICompositeRepository repository)
    {
        _repository = repository;
        _dispatchToSimDroneGateway = new DispatchToSimDroneGateway(repository);
        _homeLocation = new GeoLocation
        {
            Latitude = 39.74364421910773m,
            Longitude = -105.00858710385576m
        };
        _timer = new Timer(IssueAssignments, null, 0, RefreshInterval);
        _stopwatch = new Stopwatch();
        _stopwatch.Start();
    }

    [HttpPost("Ping")]
    public async Task<string> Ping(PingDto name)
    {
        var greeting = $"Hello, {name} from Dispatch";
        // Response.AppendHeader("Access-Control-Allow-Origin", "*");
        // .WriteAsJsonAsync() 
        Console.WriteLine(greeting);
        return greeting;
    }
    
    [HttpPost("AddDrone")]
    public async Task<AddDroneResponse> AddDrone(AddDroneRequest addDroneRequest)
    {
        var response = new AddDroneResponse
        {
            Success = false,
            DroneId = addDroneRequest.DroneId
        };
        Console.WriteLine($"DispatchController.AddDrone({addDroneRequest.ToJson()})");

        if(!await CanAddDrone(addDroneRequest.DroneId, addDroneRequest.DroneUrl))
        {
            Console.WriteLine("Either the DroneUrl or DroneId you are trying to use is taken by another drone.");
            return response;
        }
        var initDroneRequest = new InitDroneRequest
        {
            DroneId = addDroneRequest.DroneId,
            DroneUrl = addDroneRequest.DroneUrl
        };

        var initDroneResponse = await _dispatchToSimDroneGateway
           .InitDrone( initDroneRequest );
        Console.WriteLine($"Response from _dispatchToSimDroneGateway.InitDrone({initDroneRequest})\n\t->{{DroneUrl:{initDroneResponse?.DroneId},Okay:{initDroneResponse is {Okay: true}}}}");
        if (initDroneResponse is { Okay: false }) {
            return response;
        }

        Console.WriteLine($"Home location will be {_homeLocation}");

        var assignFleetRequest = new AssignFleetRequest
        {
            DroneId = addDroneRequest.DroneId,
            DroneIp = addDroneRequest.DroneUrl,
            DispatchUrl = "localhost:81",
            HomeLocation = _homeLocation
        };

        Console.WriteLine($"Proceeding with _dispatchToSimDroneGateway.AssignFleet({assignFleetRequest.ToJson()}");
        var  assignFleetResponse = await _dispatchToSimDroneGateway.AssignFleet(assignFleetRequest);
        var responseString = assignFleetResponse != null ? assignFleetResponse.IsInitializedAndAssigned.ToString() : "null";
        Console.WriteLine($"\t_dispatchToSimDroneGateway.AssignFleet - response -> {responseString}");

        if(assignFleetResponse is {IsInitializedAndAssigned: false})
        {
            Console.WriteLine($"FAILURE! new drone {addDroneRequest.DroneId} was not initiated.");
            return response;
        }
        Console.WriteLine($"success! Saving new drone {addDroneRequest.DroneId} to repository.");

        var droneRecord = new DroneRecord
        {
            DroneId = addDroneRequest.DroneId,
            DroneUrl = addDroneRequest.DroneUrl,
            Destination = _homeLocation,
            CurrentLocation = _homeLocation,
            HomeLocation = _homeLocation,
            DispatchUrl = addDroneRequest.DispatchUrl,
            State = DroneState.Ready
        };

        Console.WriteLine($"\n\n\n\nabout to YEET this drone record:\n{droneRecord.ToJson()}");
        await _repository.CreateDroneAsync(droneRecord);
        Console.WriteLine($"\n\nCreated a drone and assignment\n");
        response.Success = true;
        return response;
    }

    [NonAction]
    private async Task<bool> CanAddDrone(string droneId, string droneUrl)
    => (await _repository.GetDrones()).All(drone => !drone.DroneId.Equals(droneId) && !drone.DroneUrl.Equals(droneUrl));
    
    [HttpPost("CompleteOrder")]
    public async Task<CompleteOrderResponse> CompleteOrder(CompleteOrderRequest completeOrderRequest)
    {
        Console.WriteLine($"DispatchController.CompleteOrder -> {completeOrderRequest}");
        return new CompleteOrderResponse
        {
            IsAcknowledged = true,
            UpdateResult = await _repository.UpdateOrderAsync(completeOrderRequest)
        };
    }

    [NonAction]
    private async Task<List<AssignDeliveryResponse?>> AssignEnqueuedDeliveries()
    {
        var deliveryRequests = (await _repository.GenerateDeliveryRequests()).ToList();
        var responses = new List<AssignDeliveryResponse?>();
        foreach(var request in deliveryRequests)
        {
            Console.WriteLine($"new delivery of order {request.Order} to drone at url {request.DroneUrl}, {request.DroneId}");
            var droneResponse = await _dispatchToSimDroneGateway.AssignDelivery(request);

            if(droneResponse is { Success: true })
            {
                await _repository.UpdateAssignmentAsync(request.DroneId, false);
            }
            responses.Add(droneResponse);
            
        }

        return responses;
    }

    [HttpPost("PostInitialStatus")]
    public async Task PostInitialStatus(UpdateDroneStatusRequest initialStatusRequest)
    {
        Console.WriteLine($"DispatchController.PostInitialStatus -> {initialStatusRequest}");
        UpdateDroneStatus(initialStatusRequest);
    }
        
    [HttpPost("UpdateDroneStatus")]
    public async Task UpdateDroneStatus(UpdateDroneStatusRequest request)
    {
        Console.WriteLine($"DispatchController.UpdateDroneStatus -> {request.ToJson()}");
        await _repository.UpdateDroneAsync(request);
        var response = new UpdateDroneStatusResponse
        {
            DroneId = request.DroneId,
            IsCompletedSuccessfully = false
        };
        Console.WriteLine($"DispatchController.UpdateDroneStatus -> {request.ToJson()}");
        response.IsCompletedSuccessfully = true;
        await _repository.UpdateDroneAsync(request);
    }
}