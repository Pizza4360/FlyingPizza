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
    private readonly GeoLocation _homeLocation;
    private readonly Queue<AssignDeliveryRequest> _unfilledOrders;
    private readonly ICompositeRepository _repository;
    private readonly DispatchToSimDroneGateway _dispatchToSimDroneGateway;

    public DispatchController(ICompositeRepository repository)
    {
        _repository = repository;
        _dispatchToSimDroneGateway = new DispatchToSimDroneGateway(repository);
        _unfilledOrders = new Queue<AssignDeliveryRequest>();
        _homeLocation = new GeoLocation
        {
            Latitude = 39.74364421910773m,
            Longitude = -105.00858710385576m
        };
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
        Console.WriteLine($"DispatchController.AddDrone({addDroneRequest})");

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

        var assignFleetRequest = new AssignFleetRequest
        {
            DroneId = addDroneRequest.DroneId,
            DroneIp = addDroneRequest.DroneUrl,
            DispatchIp = addDroneRequest.DispatchUrl,
            HomeLocation = addDroneRequest.HomeLocation
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
            State = assignFleetResponse.FirstState
        };

        Console.WriteLine($"\n\n\n\nabout to YEET this drone record:\n{droneRecord.ToJson()}");
        var (record, assignment) = await _repository.CreateDroneAsync(droneRecord);
        Console.WriteLine($"\n\nResponse from the database:\ndrone record: {record}\nassignment:{assignment}\n");
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

    [HttpPost("EnqueueOrder")]
    public async Task<EnqueueOrderResponse?> EnqueueOrder(EnqueueOrderRequest enqueueOrderRequest)
    {
        Console.WriteLine($"DispatchController.EnqueueOrder -> {enqueueOrderRequest}");
        await _repository.EnqueueOrder(enqueueOrderRequest.Order);

        return new EnqueueOrderResponse
        {
            IsEnqueued = true
        };
    }

    [HttpPost("PostInitialStatus")]
    public async Task<UpdateDroneStatusResponse> PostInitialStatus(UpdateDroneStatusRequest initialStatusRequest)
    {
        Console.WriteLine($"DispatchController.PostInitialStatus -> {initialStatusRequest}");
        return await UpdateDroneStatus(initialStatusRequest);
    }
        
    [HttpPost("UpdateDroneStatus")]
    public async Task<UpdateDroneStatusResponse> UpdateDroneStatus(UpdateDroneStatusRequest droneStatusRequest)
    {
        Console.WriteLine($"DispatchController.UpdateDroneStatus -> {droneStatusRequest.ToJson()}");
        var droneRecord = new DroneRecord
        {
            DroneId = droneStatusRequest.DroneId,
            CurrentLocation = droneStatusRequest.CurrentLocation,
            State = droneStatusRequest.State
        };
        await _repository.UpdateDroneAsync(droneStatusRequest);

        var response = new UpdateDroneStatusResponse
        {
            DroneId = droneStatusRequest.DroneId,
            IsCompletedSuccessfully = false
        };

        Console.WriteLine($"DispatchController.UpdateDroneStatus -> {droneStatusRequest.ToJson()}");
        if (droneStatusRequest.State != DroneState.Ready ||
            _unfilledOrders.Count <= 0)
        {
            Console.WriteLine($"\n\n\nDrone {droneStatusRequest.DroneId} is still delivering an order. Updating the status tho....");
            response.IsCompletedSuccessfully = true;
            response.UpdateResult = await _repository.UpdateDroneAsync(droneStatusRequest);
            Console.WriteLine($"The status of {droneStatusRequest.DroneId}'s db update is {response.IsCompletedSuccessfully}\n\n\n");
        }
        else
        {
            var orderDto = _unfilledOrders.Dequeue();
            Console.WriteLine($"Drone i{droneStatusRequest.DroneId} is ready for the next order, and we have more. Resending to order {orderDto.Order}\n\n\n");
            orderDto.DroneUrl = droneStatusRequest.DroneId;
            response.IsCompletedSuccessfully = true;
            response.UpdateResult = await _repository.UpdateDroneAsync(droneStatusRequest);
        }
        return response;
    }

        
    [HttpPost("{id:length(24)}")]
    public async Task<ActionResult<DroneRecord>> GetDroneById(string requestedDroneId, string id)
    {
        Console.WriteLine($"DispatchController.Get -> {requestedDroneId}");
        var droneRecord = await _repository.GetDroneByIdAsync(requestedDroneId);

        if (droneRecord is null)
        {
            return NotFound();
        }
        return droneRecord;
    }
}