using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;
using Domain.DTO.FrontEndDispatchCommunication;
using Domain.Entities;
using Domain.GatewayDefinitions;
using Domain.RepositoryDefinitions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace Dispatch.Controllers;

[ApiController]
[Route("[controller]")]
public class DispatchController : ControllerBase
{
    private IDispatchToSimDroneGateway _dispatchToSimDroneGateway;

    private readonly IDroneRepository _drone;
    private readonly IDeliveriesRepository _deliveries;
    private readonly GeoLocation _homeLocation;
    private readonly string _dispatchUrl;
    private bool _isInitiatingDrone;

    public DispatchController(IODDSSettings settings)
    {
        _drone = settings.GetFleetCollection();
        _deliveries = settings.GetDeliveriesCollection();
        _homeLocation = settings.GetHomeLocation();
        _dispatchUrl = settings.GetDispatchUrl();
        _dispatchToSimDroneGateway = new DispatchToSimDroneGateway(_drone);
    }

    [HttpPost("Recover")]
    public async Task<bool> Revive(DroneEntity entity)
    {
        var unsuccessful = true;
        Console.WriteLine("\n\nGot a request to revive a drone.");
        var possibleDrones = (await _drone.GetAllAsync())
            .Where(x => x.DroneId.Equals(entity.DroneId)).ToList();
        
        Console.WriteLine("Possible drones are: ["
                          + $"{string.Join(",", possibleDrones)}]");
        
        if (!possibleDrones.Any()) {
            Console.WriteLine($"All my drones are active right now.... Super"
                              + $" suspicious...\ndrone -> {entity.ToJson()}");
        } 
        else if (possibleDrones.Count > 1)
        {
            Console.WriteLine("ID is already in use by a drone... Super "
                              + "ambiguous and suspicious...\n"
                              + "drone -> {model.ToJson()}");
        }
        else
        {
            Console.WriteLine($"\nDrone matched {entity.DroneId}. "
                              + $"Reviving drone.");
            entity.LatestStatus = DroneStatus.Ready;
            await _drone.UpdateAsync(entity.Update());
            unsuccessful = false;
        }
        return unsuccessful;
    }

    [HttpPost("AssignmentCheck")]
    public async Task<BaseDto> AssignmentCheck(BaseDto p)
    {
        await AddAllNewDrones();
        
        if (_isInitiatingDrone) { return p; }
        
        var deliveries = (
            from o in await GetUnfulfilledDeliveries() 
            where string.IsNullOrEmpty(o.DroneId) 
            select o
        ).ToList();
        
        var availableDrones = (
            from d in await GetAvailableDrones() 
            where string.IsNullOrEmpty(d.DeliveryId)
            select d
        ).ToList();
        
        var unfulfilledDeliveries = await GetUnfulfilledDeliveries();
        var joinDrones = string.Join(", ", availableDrones.Select(x
             => x.DroneUrl));
        
        Console.WriteLine($"Drones available: [{joinDrones}]");
        
        var enumerableDeliveries = unfulfilledDeliveries.ToList();
        var ids = string.Join(", ", enumerableDeliveries.Select(x 
            => x.DeliveryId));
        
        Console.WriteLine($"Deliveries waiting: [{ids}]");
        
        foreach (var (drone, delivery) in 
                 availableDrones.Zip(enumerableDeliveries))
        {
            var assignment = new AssignDeliveryRequest
            {
                DroneId = drone.DroneId,
                DeliveryId = delivery.DeliveryId,
                DeliveryLocation = delivery.DeliveryLocation
            };
            /*do not await!!!*/
            InitiateDelivery(assignment);
        }
        return p;
    }

    [NonAction]
    private async Task AddAllNewDrones()
    {
        var newDrones = (await _drone.GetAllAsync())
            .Where(d => d.LatestStatus == DroneStatus.Unititialized);
        
        foreach (var drone in newDrones) { await AddDrone(drone); }
    }

    [NonAction]
    public async Task AddDrone(DroneEntity drone)
    {
        _isInitiatingDrone = true;
        drone.DispatchUrl = _dispatchUrl;
        drone.HomeLocation = _homeLocation;
        
        Console.WriteLine($"DISPATCH_URL = {_dispatchUrl}");
        
        var initDroneRequest = new InitDroneRequest
        {
            DroneId = drone.DroneId,
            DroneUrl = drone.DroneUrl
        };

        var initDroneResponse = _dispatchToSimDroneGateway
            .InitDrone(initDroneRequest).Result;
        
        Console.WriteLine(
            "\n\n\n\nResponse from _dispatchToSimDroneGateway.InitDrone("
            + $"{initDroneRequest})\n\t->{{DroneId:{initDroneResponse?.DroneId}"
            + $",Okay:{initDroneResponse?.Okay}}}\n\n\n\n");

        if (initDroneResponse is { Okay: false })
        {
            return;
        }

        var assignFleetRequest = new AssignFleetRequest
        {
            DroneId = drone.DroneId,
            DroneUrl = drone.DroneUrl,
            DispatchUrl = _dispatchUrl,
            HomeLocation = drone.HomeLocation,
            BadgeNumber = drone.BadgeNumber
        };

        Console.WriteLine("\n\n\n\nProceeding with _dispatchToSimDroneGateway"
                          + $".AssignFleet({assignFleetRequest.ToJson()}"
                          + "\n\n\n\n");
       
        var assignFleetResponse = _dispatchToSimDroneGateway
            .AssignFleet(assignFleetRequest).Result;
        
        var responseString = assignFleetResponse != null
            ? assignFleetResponse.IsInitializedAndAssigned.ToString()
            : "null";
        
        Console.WriteLine("\n\n\n\n_dispatchToSimDroneGateway.AssignFleet"
                          + $" - response -> {responseString}\n\n\n\n");

        if (assignFleetResponse is {IsInitializedAndAssigned: false})
        {
            Console.WriteLine($"FAILURE! new drone {drone.DroneId} was not"
                              + " initiated.");
        }

        Console.WriteLine($"\n\n\n\nsuccess! Saving new drone {drone.DroneId}"
                          + " to repository.\n\n\n\n");
        
        drone.LatestStatus = DroneStatus.Ready;
        drone.DeliveryId = "";
        drone.DispatchUrl = _dispatchUrl;
        await _drone.UpdateAsync(drone.Update());

        Console.WriteLine("\n\n\n\nabout to YEET this drone model:\n"
                          + $"{drone.ToJson()}");
    }

    [HttpPost("CompleteDelivery")]
    public async Task<CompleteDeliveryResponse> 
        CompleteDelivery(CompleteDeliveryRequest request)
    {
        var delivery = new DeliveryEntity
        {
            DeliveryId = request.DeliveryId,
            Status = Deliveriestatus.Delivered,
            TimeDelivered = request.Time,
            DroneId = request.DroneId
        };
        
        Console.WriteLine(
            $"delivery {delivery.DeliveryId} has been delivered?"
            + $" {delivery.HasBeenDelivered}"
            + $" @ time {delivery.TimeDelivered}");
        
        var result = await _deliveries.UpdateAsync(delivery.Update());
        
        return new CompleteDeliveryResponse
        {
            IsAcknowledged = result.IsAcknowledged  && result.ModifiedCount == 1
        };
    }

    [HttpPost("AssignDelivery")]
    private async Task<AssignDeliveryResponse> InitiateDelivery(
        AssignDeliveryRequest request)
    {
        Console.WriteLine($"In DispatchController.AssignDeliveryResponse, "
                          + $"delivery :{request.ToJson()}");
        Console.WriteLine("!!!!!!" + request.ToJson());

        var delivery = await _deliveries.GetByIdAsync(request.DeliveryId);
        var drone = await _drone.GetByIdAsync(request.DroneId);

        var s1 = $"drone.DeliveryId {drone.DeliveryId} -> ";
        var s2 = $"delivery.DroneId {delivery.DroneId} -> ";
        
        delivery.DroneId = drone.DroneId;
        drone.DeliveryId = delivery.DeliveryId;
        
        s1 += $"{drone.DeliveryId}";
        s2 += $"{delivery.DroneId}";
        
        Console.WriteLine($"{s1}\n{s2}");
        
        delivery.Status = Deliveriestatus.Assigned;
        drone.LatestStatus = DroneStatus.Assigned;

        await _drone.UpdateAsync(drone.Update());
        await _deliveries.UpdateAsync(delivery.Update());

        return await _dispatchToSimDroneGateway.AssignDelivery(request);
    }


    [NonAction]
    private async Task<IEnumerable<DeliveryEntity>> GetUnfulfilledDeliveries()
    {
        var deliveries = await _deliveries.GetAllAsync();
        
        Console.WriteLine($"All the deliveries: {deliveries.Count}");
        
        var unassignedDeliveries = deliveries.Where(o =>
            !o.HasBeenDelivered 
            && o is {Status: Deliveriestatus.Waiting}
            && string.IsNullOrEmpty(o.DroneId));
        
        return unassignedDeliveries;
    }

    [NonAction]
    private async Task<IEnumerable<DroneEntity>> GetAvailableDrones()
    {
        if (_isInitiatingDrone)
        {
            Console.WriteLine("Thread lock, cannot read drones at this time.");
            return new List<DroneEntity>();
        }
        
        try
        {
            Console.WriteLine("DequeueDeliveries...");
            
            var drones = 
                from d in await _drone.GetAllAsync()
                where d is { LatestStatus: DroneStatus.Ready } 
                      && string.IsNullOrEmpty(d.DeliveryId)
                select d;
            
            var availableDrones = new List<DroneEntity>();
            foreach (var drone in drones)
            {
                if (!await _dispatchToSimDroneGateway.HealthCheck(
                        drone.DroneId)) { continue; }
                
                Console.WriteLine($"drone at {drone.DroneUrl} is healthy");
                
                availableDrones.Add(drone);
            }
            return availableDrones;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new List<DroneEntity>();
        }
    }

    [HttpPost("PostInitialStatus")]
    public async Task<UpdateDroneStatusResponse> PostInitialStatus(
        DroneUpdate initialStatusRequest)
    {
        Console.WriteLine($"DispatchController.PostInitialStatus ->'"
                          + $" {initialStatusRequest}");
        
        return await UpdateDroneStatus(initialStatusRequest);
    }

    [HttpPost("UpdateDroneStatus")]
    public async Task<UpdateDroneStatusResponse> UpdateDroneStatus(
        DroneUpdate droneStatusRequest)
    {
        Console.WriteLine($"DispatchController.UpdateDroneStatus ->"
                          + $" {droneStatusRequest.ToJson()}");
        
        var updateResult = await _drone.UpdateAsync(droneStatusRequest);

        var success = updateResult.IsAcknowledged 
                      && updateResult.ModifiedCount == 1;
        
        var response = new UpdateDroneStatusResponse
        {
            DroneId = droneStatusRequest.DroneId,
            IsCompletedSuccessfully = success
        };
        
        Console.WriteLine($"DispatchController.UpdateDroneStatus -> "
                          + $"{droneStatusRequest.ToJson()}");

        return response;
    }


    [HttpPost("{id:length(24)}")]
    public async Task<ActionResult<DroneEntity>> GetDroneById(
        string id
    )
    {
        Console.WriteLine($"DispatchController.Get -> {id}");
        var droneModel = await _drone.GetByIdAsync(id);

        if (droneModel is null) { return NotFound(); }

        return droneModel;
    }
    [NonAction]
    public void ChangeGateway(IDispatchToSimDroneGateway mockedGate)
    {
        _dispatchToSimDroneGateway = mockedGate;
    }
}