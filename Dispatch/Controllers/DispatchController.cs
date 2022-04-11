using Dispatch.Services;
using Domain;
using Domain.DTO.DroneDispatchCommunication;
using Domain.DTO.Shared;
using Domain.Entities;
using Domain.InterfaceImplementations.Gateways;
using Domain.InterfaceImplementations.Repositories;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Dispatch.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DispatchController : ControllerBase
    {
        [HttpPost("Ping")]
        public async Task<string> Ping(string name)
        {
            Console.WriteLine("made it here");
            var s = $"hello, {name}";
            return s;
        }
        // Step 1, use use DispatchToDroneGateway to init registration
        [HttpPost("AddDrone")]
        public Task<bool> AddDrone(GatewayDto dto)
        {
            Console.WriteLine($"{dto.Url}");
            return Task.FromResult(_dispatchToDroneGateway.InitializeRegistration(
                    dto.Url
                    , dto.Url
                    , GetNewBadgeNumber())
                .Result);
        }

        private int GetNewBadgeNumber()
        {
            return 5;
            // Todo, look in the database and get the next badgenumber
        }

        // Step 5, receive the first POST status update, send it to the database,
        // then use DispatchToDroneGateway to supply a badge number and
        // home location to drone
        [HttpPost("SendInitialStatus")]
        public async Task<bool> CompleteRegistration(InitDrone dto)
        {
            Console.WriteLine($"{dto}");
            await PatchDroneStatus(dto.FistStatusUpdateRequestUpdate);
            return _dispatchToDroneGateway.CompleteRegistration(dto.Record)
                .Result.Content.Headers.ToString()
                .Contains("hello, world");
        }


        [HttpGet("badge_request")]
        public Task<int> BadgeResponse()
        {
            return Task.FromResult(0);
        }


        private readonly FleetRepository _droneRepo;
        private readonly OrderRepository _orderRepo;

        private readonly DispatchToDroneGateway _dispatchToDroneGateway;

        // private readonly ILogger<DispatchController> _logger;
        private readonly GeoLocation _homeLocation;
        private readonly Queue<AssignDeliveryRequest> _unfilledOrders;

        public DispatchController(
        FleetRepository droneRepo,
        // DroneGateway droneGateway,
        OrderRepository orderRepo
        // GeoLocation homeLocation,
        // Queue<AssignDeliveryRequest> unfilledOrders
        )
        {
            _droneRepo = droneRepo;
            _orderRepo = orderRepo;
            _dispatchToDroneGateway = new DispatchToDroneGateway();
            _unfilledOrders = new Queue<AssignDeliveryRequest>();
            _dispatchToDroneGateway.IdToIpMap = _droneRepo.GetAllAddresses().Result;
            _homeLocation = new GeoLocation
            {
                Latitude = 39.74364421910773m
                , Longitude = -105.00858710385576m
            };
        }


        /// <summary>
        /// A drone will call this method when their assigned order has been completed.
        /// The delivery time of the order will be patched.
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        [HttpPatch("CompleteOrder")]
        public Task<bool>
            PatchDeliveryTime(CompleteOrderRequest completeOrder)
            => Task.FromResult(
                _orderRepo.PatchTimeCompleted(completeOrder.OrderId)
                .Result);

        /*
        /// <summary>
        /// This method is invoked from the front to add a new drone to the fleet.
        /// </summary>
        /// <param name="request"></param>
        /// <returns>`true` only if the handshake is completed and the drone is initialized.</returns>
        [HttpPost("StartFleetRegistration")]
        public async Task<bool> StartFleetRegistration(InitDroneRequest request)
        {
            Console.WriteLine($"received \"{request}\"" +
                "Attempting to initialize communication with drone...");
            var canBeInitialized
                = _dispatchToDroneGateway.StartRegistration(
                    $"{request.Url}/Drone/init_registration");
            if (!canBeInitialized.IsCompletedSuccessfully)
            {
                Console.WriteLine("The drone could not be initialized...");
                return false;
            }

            // Todo make a new guid and make sure it is different from all other drones
            var newDrone = new DroneRecord
            {
                BadgeNumber = request.BadgeNumber
                , IpAddress = request.Url
                , HomeLocation = _homeLocation
                , DispatcherUrl = "//172.18.0.0:4000"
                , Destination = _homeLocation
                , CurrentLocation = _homeLocation
                , OrderId = ""
                , State = DroneState.Charging
                , Id = "abcdefg"
            };

            var response = await _dispatchToDroneGateway.AssignToFleet(
                newDrone.IpAddress
                , newDrone.BadgeNumber
                , newDrone.DispatcherUrl
                , newDrone.HomeLocation);
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(
                    "Drone is online, but there was a problem assigning to fleet. either the drone is not ready to be initialized or is already part of a fleet.");
                return false;
            }

            await _droneRepo.CreateAsync(newDrone);
            // Todo dispatcher saves handshake record to DB
            return true;
        }
        */


        [HttpPost("PostInitialStatus")]
        public UpdateResult
            Post(DroneStatusUpdateRequest stateDto) 
            => PatchDroneStatus(stateDto).Result;

        /// If a drone updates its status, patch its status.
        /// Then check if there is an enqueued order. If so,
        /// it should be assigned to this drone.
        [HttpPatch("PatchDroneStatus")]
        public async Task<UpdateResult> PatchDroneStatus(DroneStatusUpdateRequest stateDto)
        {
            if (stateDto.State != DroneState.Ready ||
                _unfilledOrders.Count <= 0)
            {
                return _droneRepo.PatchDroneStatus(stateDto).Result;
            }
            var orderDto = _unfilledOrders.Dequeue();
            await _dispatchToDroneGateway.AssignDelivery(
                stateDto.Id
                , orderDto.OrderId
                , orderDto.OrderLocation);

            return _droneRepo.PatchDroneStatus(stateDto)
                .Result;
        }

        /// <summary>
        /// For testing
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:length(24)}")]
        public async Task<ActionResult<DroneRecord>>
            Get(string id)
        {
            var droneRecord = await _droneRepo.GetAsync(id);

            if (droneRecord is null)
            {
                return NotFound();
            }

            return droneRecord;
        }
    }
}

