using Domain;
using Domain.Entities;
using Domain.Interfaces.Gateways;
using Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.DTO.DroneCommunicationDto.DispatcherToDrone;
using Domain.DTO.DroneCommunicationDto.DroneToDispatcher;
using Domain.DTO.FrontEndDispatchCommunication.FrontEndToDispatcher;

namespace DroneDispatcher.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DispatcherController : ControllerBase
    {
        private readonly IDronesRepository _dronesRepository;
        private readonly IOrdersRepository _ordersRepository;
        private readonly IDroneGateway _droneGateway;
        private readonly Queue<Order> _unfilledOrders;
        public GeoLocation Home { get;}
        // TODO: added since dispatcher should know where it starts?

        public DispatcherController(
            IDronesRepository droneRepository,
            // IOrdersRepository ordersRepository,
            IDroneGateway droneGateway
            )
        {
            _dronesRepository = droneRepository;
            // _ordersRepository = ordersRepository;
            _droneGateway = droneGateway;
            _unfilledOrders = new Queue<Order>();
            Home = new GeoLocation
            {
                Latitude = 39.74364421910773m,
                Longitude = -105.00561147600774m
            };
        }

        #region endpoints
        [HttpPost("register")]
        public async Task<IActionResult> RegisterNewDrone(DroneRegistrationInfo droneInfo)
        {
            Console.WriteLine("We are gonna register some shit!!");
            // Todo make a new guid and make sure it is different from all other drones
            var newDrone = new Drone
            {
                BadgeNumber = droneInfo.BadgeNumber,
                IpAddress = droneInfo.IpAddress,
                // TODO: added since required elsewhere in the handshake, may not be ideal
                HomeLocation = Home,
                DispatcherUrl = "http://localhost:4000",
                Destination = Home,
                CurrentLocation = Home,
                OrderId = "",
                Status = "Ready",
                Id = ""
            };
            
            // Register drone w/ dispatcher by doing the following:
            // wait for OK message back from drone
            
                var response = await _droneGateway.StartRegistration(newDrone.IpAddress, newDrone.BadgeNumber,
                    newDrone.DispatcherUrl, newDrone.HomeLocation);
                if (!response)
                    return Problem("either the drone is not ready to be initialized or is already part of a fleet.");
                // Todod: save drone to database
                await _dronesRepository.CreateAsync(newDrone);
                // Todo dispatcher saves handshake record to DB
                    
                // Todod dispatcher sends OK back to drone so that drone can stop waiting and start updating status
                await _droneGateway.OKToSendStatus(newDrone.IpAddress);
                return Ok();
        }

        [HttpPost("add_order")]
        public async Task<IActionResult> AddNewOrder(AddOrderDTO order)
        {
            Console.WriteLine("adding a new order");
            var didSucceed = false;
            //Todo, eventually availableDrones needs to come from the db 
            // var availableDrones = await _dronesRepository.GetAllAvailableDronesAsync();
            var availableDrones = new List<Drone>{
                new Drone
                {
                    IpAddress = "localhost:5001",
                    Id = "1",
                    BadgeNumber = new Guid(),
                    OrderId = "",
                    HomeLocation = Home,
                    CurrentLocation = Home,
                    Status = "ready",
                    DispatcherUrl = "localhost:4000"
                }
            };
            var newOrder = new Order
            {
                Id = order.Id,
                DeliveryLocation = new GeoLocation
                {
                    Latitude = 39.736134990245326m,
                    Longitude = -104.99060497415945m
                }
            };
            if (availableDrones.Any())
            {
                //Todo, this needs to actually come from the repo one day...
                // var newOrder = await _ordersRepository.GetByIdAsync(orderId);
                    didSucceed = await _droneGateway.AssignDelivery(availableDrones.First().IpAddress, newOrder.Id, newOrder.DeliveryLocation);
            }
            else
            {
                _unfilledOrders.Enqueue(newOrder);
                didSucceed = true;
            }

            // TODO: unhappy path

            return Ok("aight");
        }

        [HttpPost("complete_delivery")]
        public async Task<IActionResult> CompleteDelivery(string orderNumber)
        {
            var order = await _ordersRepository.GetByIdAsync(orderNumber);
            order.TimeDelivered = DateTimeOffset.UtcNow;
            await _ordersRepository.Update(order);

            return Ok();
        }

        [HttpPost("ready_for_order")]
        public async Task<IActionResult> DroneIsReadyForOrder(string droneId)
        {
            if (_unfilledOrders.Count() > 0)
            {
                var droneIpAddress = (await _dronesRepository.GetByIdAsync(droneId)).IpAddress;
                var nextOrder = _unfilledOrders.Dequeue();
                var didSucceed = await _droneGateway.AssignDelivery(droneIpAddress, nextOrder.Id, nextOrder.DeliveryLocation);

                // TODO: Unhappy Path
            }

            return Ok();
        }
        #endregion endpoints

        [HttpPost("update_status")]
        public async Task<IActionResult> UpdateStatus(UpdateStatusDto dto)
        {
            Console.WriteLine($"putting:\n{dto}");
            //Todo need to put to db...
            return Ok();
        }
    }
}
