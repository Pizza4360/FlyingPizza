using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;
using Domain.DTO.FrontEndDispatchCommunication;
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
        public async Task<AddDroneResponse> AddDrone(AddDroneRequest dto)
        {
            var initResponse = _dispatchToDroneGateway
                .InitializeRegistration(
                    new InitDroneRequest
                    {
                        DroneId = dto.DroneId,
                        DroneIp = dto.DroneIp
                    }
                );
            
            if (!initResponse.Okay)
            {
                return new AddDroneResponse { BadgeNumber = dto.BadgeNumber, Success = false };
            }

            var assignFleetResponse = _dispatchToDroneGateway.AssignFleet(new AssignFleetRequest
            {
                BadgeNumber = dto.BadgeNumber,
                DispatcherIp = dto.DispatchIp,
                DroneIp = dto.DroneIp,
                HomeLocation = dto.HomeLocation,
                DroneId = dto.DroneId
            });
            await _droneRepo.CreateAsync(
                new DroneRecord
                {
                    OrderId = null,
                    Id = dto.DroneId,
                    DroneIp = dto.DroneIp,
                    BadgeNumber = dto.BadgeNumber,
                    Destination = dto.HomeLocation,
                    CurrentLocation = dto.HomeLocation,
                    HomeLocation = dto.HomeLocation,
                    DispatcherUrl = dto.DispatchIp,
                    State = assignFleetResponse.FirstState
                });
            if (assignFleetResponse.IsInitializedAndAssigned)
            {
                _dispatchToDroneGateway.AddIdToIpMapping(dto.DroneId, dto.DroneIp);
            }
            return new AddDroneResponse
            {
                BadgeNumber = dto.BadgeNumber,
                Success = assignFleetResponse.IsInitializedAndAssigned
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

        [HttpPost("EnqueueOrder")]
        public AssignDeliveryResponse EnqueueOrder(AssignDeliveryRequest request)
        {
            List<DroneRecord> availableDrones;
            do
            {
                Thread.Sleep(3000);
                availableDrones = _droneRepo.GetAllAsync()
                    .Result;
            }
            while (availableDrones.Count == 0);
            var id = availableDrones.First().Id;
            return _dispatchToDroneGateway.AssignDelivery(new AssignDeliveryRequest
            {
                DroneId = id,
                OrderId = request.OrderId,
                OrderLocation = request.OrderLocation
            });
        }

        [HttpPost("PostInitialStatus")]
        public async Task<bool> Post(DroneStatusUpdateRequest stateDto) =>
            await PatchDroneStatus(stateDto);

        /// If a drone updates its status, patch its status.
        /// Then check if there is an enqueued order. If so,
        /// it should be assigned to this drone.
        [HttpPatch("PatchDroneStatus")]
        public async Task<bool> PatchDroneStatus(DroneStatusUpdateRequest stateDto)
        {
            var droneRecord = new DroneRecord
            {
                Id = stateDto.Id,
                CurrentLocation = stateDto.Location,
                State = stateDto.State
            };

            if (stateDto.State != DroneState.Ready ||
                _unfilledOrders.Count <= 0)
            {
                return await _droneRepo.UpdateAsync(droneRecord);
            }
            var orderDto = _unfilledOrders.Dequeue();
            return await _droneRepo.UpdateAsync(droneRecord);
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
            var droneRecord = await _droneRepo.GetByIdAsync(id);

            if (droneRecord is null)
            {
                return NotFound();
            }

            return droneRecord;
        }
    }
}

