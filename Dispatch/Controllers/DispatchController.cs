using Dispatch.Services;
using Domain;
using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;
using Domain.Entities;
using Domain.Gateways;
using Microsoft.AspNetCore.Mvc;
using Order = Domain.Entities.Order;

namespace Dispatch.Controllers
{

    [ApiController]
    [Route("[controller]")]
    public class DispatchController : ControllerBase
    {


        // Step 1, use use DispatchToDroneGateway to init registration
        [HttpPost("add_drone")]
        public async Task<bool> AddDrone(GatewaysDto dto)
        {
            _dispatchToDroneGateway = dto.DispatchToDroneGateway;
            return _dispatchToDroneGateway.InitRegistration(
                    dto.Url
                    , dto.DroneToDispatchGateway,
                    GetNewBadgeNumber())
                .Result;
        }

        private int GetNewBadgeNumber()
        {
            return 5;
            // Todo, look in the database and get the next badgenumber
        }

        // Step 5, receive the first POST status update, send it to the database,
        // then use DispatchToDroneGateway to supply a badge number and
        // home location to drone
        [HttpPost("send_init_status")]
        public async Task<bool> CompleteRegistration(Domain.DTO.InitDrone dto)
        {
            await PatchDroneStatus(dto.FistStatusUpdate);
            return _dispatchToDroneGateway.CompleteRegistration(dto.Record)
                .Result.Content.Headers.ToString()
                .Contains("hello, world");
        }



        [HttpGet("badge_request")]
        public async Task<int> BadgeResponse()
        {
            return 0;
        }


        private readonly FleetService _droneRepo;
        private readonly OrdersService _orderRepo;

        private DispatchToDroneGateway _dispatchToDroneGateway;

        // private readonly ILogger<DispatchController> _logger;
        private readonly GeoLocation _homeLocation;
        private readonly Queue<Delivery> _unfilledOrders;

        /// <summary>
        /// Call this method for debugging and testing if the dispatcher
        /// is alive.
        /// </summary>
        /// <returns></returns>
        [HttpPost("ping")]
        public string Ping() => "I'm alive!";

        public DispatchController(
        FleetService droneRepo,
        // DroneGateway droneGateway,
        OrdersService orderRepo
        // GeoLocation homeLocation,
        // Queue<Delivery> unfilledOrders
        )
        {
            _droneRepo = droneRepo;
            _orderRepo = orderRepo;
            _dispatchToDroneGateway = new DispatchToDroneGateway();
            _unfilledOrders = new Queue<Delivery>();
            _dispatchToDroneGateway.IdToIpMap = _droneRepo.GetAllIpAddresses()
                .Result;
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
        [HttpPatch("complete_order")]
        public async Task<bool>
            PatchDeliveryTime(Order order)
            => _orderRepo.PatchTimeCompleted(order.ID)
                .Result;

        /// <summary>
        /// This method is invoked from the front to add a new drone to the fleet.
        /// </summary>
        /// <param name="dto"></param>
        /// <returns>`true` only if the handshake is completed and the drone is initialized.</returns>
        [HttpPost("register")]
        public async Task<bool> StartFleetRegistration(InitGatewayPost dto)
        {
            Console.WriteLine(
                "Attempting to initialize communication with drone...");
            var canBeInitialized
                = _dispatchToDroneGateway.StartRegistration(
                    $"{dto.Url}/Drone/init_registration");
            if (!canBeInitialized.IsCompletedSuccessfully)
            {
                Console.WriteLine("The drone could not be initialized...");
                return false;
            }

            // Todo make a new guid and make sure it is different from all other drones
            var newDrone = new DroneRecord
            {
                BadgeNumber = dto.BadgeNumber
                , IpAddress = dto.Url
                , HomeLocation = _homeLocation
                , DispatcherUrl = "//172.18.0.0:4000"
                , Destination = _homeLocation
                , CurrentLocation = _homeLocation
                , OrderId = ""
                , State = DroneState.Charging
                , ID = "abcdefg"
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


        [HttpPost("first_status")]
        public async Task<bool>
            Post(DroneStatusPatch stateDto)
        {
            bool firstStatusOk = PatchDroneStatus(stateDto)
                .Result;
            return firstStatusOk;
        }

        /// If a drone updates its status, patch its status.
        /// Then check if there is an enqueued order. If so,
        /// it should be assigned to this drone.
        [HttpPatch("update_status")]
        public async Task<bool>
            PatchDroneStatus(DroneStatusPatch stateDto)
        {
            if (stateDto.State == DroneState.Ready &&
                _unfilledOrders.Count > 0)
            {
                var orderDto = _unfilledOrders.Dequeue();
                _dispatchToDroneGateway.AssignDelivery(
                    stateDto.Id
                    , orderDto.OrderId
                    , orderDto.OrderLocation);
            }

            return _orderRepo.PatchDroneStatus(stateDto)
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

namespace Domain.DTO
{
    public class GatewaysDto
    {
        public DroneToDispatchGateway DroneToDispatchGateway { get; set; }
        public DispatchToDroneGateway DispatchToDroneGateway { get; set; }
        public string Url { get; set; }
    }

    public class InitDrone
    {
        public DroneRecord Record { get; set; }
        public DroneStatusPatch FistStatusUpdate { get; set; } 
    }
}
