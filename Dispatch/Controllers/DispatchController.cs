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
    private readonly IFleetRepository _droneRepo;
    private readonly IOrdersRepository _orderRepo;
    private readonly DispatchToSimDroneGateway _dispatchToSimDroneGateway;
        
    public DispatchController(
        IFleetRepository droneRepo,
        // DroneGateway droneGateway,
        IOrdersRepository orderRepo
        // GeoLocation homeLocation,
        // Queue<AssignDeliveryRequest> unfilledOrders
        )
    {
        _droneRepo = droneRepo;
        _orderRepo = orderRepo;
        _dispatchToSimDroneGateway = new DispatchToSimDroneGateway(_droneRepo.GetAllAddresses().Result, 84);
        _unfilledOrders = new Queue<AssignDeliveryRequest>();
        _homeLocation = new GeoLocation
        {
            Latitude = 39.74364421910773m,
            Longitude = -105.00858710385576m
        };
    }

        
    [HttpPost("Ping")]
    public Task<string> Ping(PingDto name)
    {
        var greeting = $"Hello, {name} from Dispatch";
        // Response.AppendHeader("Access-Control-Allow-Origin", "*");
        // .WriteAsJsonAsync() 
        Console.WriteLine(greeting);
        return Task.FromResult(greeting);
    }
        
    // Step 1, use use DispatchToSimDroneGateway to init registration
    [HttpPost("AddDrone")]
    public AddDroneResponse AddDrone(AddDroneRequest addDroneRequest)
    {
        Console.WriteLine($"DispatchController.AddDrone({addDroneRequest})");

        var initDroneRequest = new InitDroneRequest
        {
            DroneId = addDroneRequest.DroneId,
            DroneIp = addDroneRequest.DroneIp
        };

        var initDroneResponse = _dispatchToSimDroneGateway
           .InitDrone( initDroneRequest );
        //{"DroneId":"6262a69f30b68bca89360954", "DroneIp":"http://localhost:5103","DispatchIp":"http://localhost:5102","HomeLocation":,"DispatchIp":3fa85f64-5717-4562-b3fc-2c963f66afa6,
        Console.WriteLine($"Response from _dispatchToSimDroneGateway.InitDrone({initDroneRequest})\n\t->{{DroneId:{initDroneResponse.DroneId},Okay:{initDroneResponse.Okay}}}");
        if (!initDroneResponse.Okay)
        {
            return new AddDroneResponse { BadgeNumber = addDroneRequest.BadgeNumber, Success = false };
        }

        var assignFleetRequest = new AssignFleetRequest
        {
            DroneId = addDroneRequest.DroneId,
            DroneIp = addDroneRequest.DroneIp,
            DispatchIp = addDroneRequest.DispatchIp,
            BadgeNumber = addDroneRequest.BadgeNumber,
            HomeLocation = addDroneRequest.HomeLocation
        };
        /*
         { "DroneId": "6262a69f30b68bca89360954", "BadgeNumber": "7fa8bdf7-cfb2-4b55-80e0-62790dd6d0a7", "HomeLocation": { "Latitude": 39.74386695629378, "Longitude": -105.00610500179027 }, "DroneIp": "http://localhost:5103", "DispatchIp": "http://localhost:5102" }
         */
        Console.WriteLine($"Proceeding with _dispatchToSimDroneGateway.AssignFleet({assignFleetRequest.ToJson()}");
        var assignFleetResponse = _dispatchToSimDroneGateway.AssignFleet(assignFleetRequest);
        var responseString = assignFleetResponse != null ? assignFleetResponse.IsInitializedAndAssigned.ToString() : "null";
        Console.WriteLine($"\t_dispatchToSimDroneGateway.AssignFleet - response -> {responseString}");

        if(assignFleetResponse is {IsInitializedAndAssigned: false})
        {
            Console.WriteLine($"FAILURE! new drone {addDroneRequest.BadgeNumber} was not initiated.");
            return new AddDroneResponse
            {
                BadgeNumber = addDroneRequest.BadgeNumber,
                Success = false
            };
        }
        Console.WriteLine($"success! Saving new drone {addDroneRequest.BadgeNumber} to repository.");
        _droneRepo.CreateAsync(
            new DroneRecord
            {
                OrderId = null,
                DroneId = addDroneRequest.DroneId,
                DroneIp = addDroneRequest.DroneIp,
                BadgeNumber = addDroneRequest.BadgeNumber,
                Destination = addDroneRequest.HomeLocation,
                CurrentLocation = addDroneRequest.HomeLocation,
                HomeLocation = addDroneRequest.HomeLocation,
                DispatchIp = addDroneRequest.DispatchIp,
                State = assignFleetResponse.FirstState
            });
        return new AddDroneResponse
        {
            BadgeNumber = addDroneRequest.BadgeNumber,
            Success = true
        };
    }

        
    [HttpPatch("CompleteOrder")]
    public async Task<bool> CompleteOrder(CompleteOrderRequest completeOrderRequest)
    {
        Console.WriteLine($"DispatchController.CompleteOrder -> {completeOrderRequest}");
        var order = new Order
        {
            DroneId = completeOrderRequest.OrderId,
            TimeDelivered = DateTime.Now
        };
        return await _orderRepo.UpdateAsync(order);
    }

    [HttpPost("EnqueueOrder")]
    public AssignDeliveryResponse? EnqueueOrder(AddOrderRequest enqueueOrderRequest)
    {
        Console.WriteLine($"DispatchController.EqueueOrder -> {enqueueOrderRequest}");
        List<DroneRecord> availableDrones;
        do
        {
            Thread.Sleep(3000);
            availableDrones = _droneRepo.GetAllAsync()
                                        .Result;
        }
        while (availableDrones.Count == 0);
        Console.WriteLine($"\n\nAvailable drones are:\n{string.Join("\n", availableDrones.Select(x => x.ToString()))}");
        var droneId = availableDrones.First().DroneId;
        return _dispatchToSimDroneGateway.AssignDelivery(new AssignDeliveryRequest
        {
            DroneId = droneId,
            OrderId = enqueueOrderRequest.OrderId,
            OrderLocation = enqueueOrderRequest.OrderLocation
        });
    }

    [HttpPost("PostInitialStatus")]
    public UpdateDroneStatusResponse PostInitialStatus(UpdateDroneStatusRequest initialStatusRequest)
    {
        Console.WriteLine($"DispatchController.PostInitialStatus -> {initialStatusRequest}");
        return UpdateDroneStatus(initialStatusRequest);
    }
        
    [HttpPatch("UpdateDroneStatus")]
    public UpdateDroneStatusResponse UpdateDroneStatus(UpdateDroneStatusRequest droneStatusRequest)
    {
        Console.WriteLine($"DispatchController.UpdateDroneStatus -> {droneStatusRequest}");
        var droneRecord = new DroneRecord
        {
            DroneId = droneStatusRequest.DroneId,
            CurrentLocation = droneStatusRequest.Location,
            State = droneStatusRequest.State
        };

        var response = new UpdateDroneStatusResponse()
        {
            DroneId = droneStatusRequest.DroneId,
            IsCompletedSuccessfully = false
        };

        if (droneStatusRequest.State != DroneState.Ready ||
            _unfilledOrders.Count <= 0)
        {
            response.IsCompletedSuccessfully = _droneRepo.UpdateAsync(droneRecord).Result;
        }
        else
        {
            var orderDto = _unfilledOrders.Dequeue();
            response.IsCompletedSuccessfully = _droneRepo.UpdateAsync(droneRecord).Result;
        }

        return response;
    }

        
    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<DroneRecord>> GetDroneById(string requestedDroneId)
    {
        Console.WriteLine($"DispatchController.Get -> {requestedDroneId}");
        var droneRecord = await _droneRepo.GetByIdAsync(requestedDroneId);

        if (droneRecord is null)
        {
            return NotFound();
        }
        return droneRecord;
    }
}