using Domain;
using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;
using Domain.DTO.FrontEndDispatchCommunication;
using Domain.Entities;
using Domain.InterfaceImplementations.Gateways;
using Domain.InterfaceImplementations.Repositories;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

namespace Dispatch.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DispatchController : ControllerBase
    {
        // private readonly ILogger<DispatchController> _logger;
        private readonly GeoLocation _homeLocation;
        private readonly Queue<AssignDeliveryRequest> _unfilledOrders;
        private readonly FleetRepository _droneRepo;
        private readonly OrderRepository _orderRepo;
        private readonly DispatchToDroneGateway _dispatchToDroneGateway;
        
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

        
        [HttpPost("Ping")]
        public Task<string> Ping(GatewayDto name)
        {
            var greeting = $"Hello, {name} from Dispatch";
            // Response.AppendHeader("Access-Control-Allow-Origin", "*");
                // .WriteAsJsonAsync() 
            Console.WriteLine(greeting);
            return Task.FromResult(greeting);
        }
        
        // Step 1, use use DispatchToDroneGateway to init registration
        [HttpPost("AddDrone")]
        public AssignFleetResponse AddDrone(AddDroneRequest dto)
        {
            var initResponse = _dispatchToDroneGateway
                .InitializeRegistration(
                    new InitDroneRequest
                    {
                        Id = dto.Id,
                        DroneIp = dto.DroneIp
                    }
                ).Result;
            
            if (!initResponse.Okay)
            {
                return new AssignFleetResponse{Id = dto.Id, Okay = false};
            }

            var assignFleetResponse = _dispatchToDroneGateway.CompleteRegistration(new AssignFleetRequest
            {
                BadgeNumber = dto.BadgeNumber,
                DispatcherIp = dto.DispatchIp,
                DroneIp = dto.DroneIp,
                HomeLocation = dto.HomeLocation,
                Id = dto.Id
            });
            _droneRepo.CreateAsync(
                new DroneRecord
                {
                    OrderId = null,
                    Id = dto.Id,
                    IpAddress = dto.DroneIp,
                    BadgeNumber = dto.BadgeNumber,
                    Destination = dto.HomeLocation,
                    CurrentLocation = dto.HomeLocation,
                    HomeLocation = dto.HomeLocation,
                    DispatcherUrl = dto.DispatchIp,
                    State = assignFleetResponse.FirstState
                });
            return assignFleetResponse;
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

        [HttpPost("AssignDelivery")]
        public AssignDeliveryResponse AssignDelivery(AssignDeliveryRequest request)
        {
            List<DroneRecord> availableDrones;
            do
            {
                Thread.Sleep(3000);
                availableDrones = _droneRepo.GetAll()
                    .Result;
            }
            while (availableDrones.Count == 0);
            

            request.DroneId = availableDrones.First().Id;
            return _dispatchToDroneGateway.AssignDelivery(request);
        }

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

