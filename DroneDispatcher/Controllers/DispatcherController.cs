using Domain.Entities;
using Domain.Interfaces.Gateways;
using Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;
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
        public readonly IOrdersRepository _ordersRepository;
        private readonly IDroneGateway _droneGateway;
        public readonly Queue<Order> _unfilledOrders;

        public GeoLocation Home { get; }
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
                DispatcherUrl = "http://172.18.0.0:4000",
                Destination = Home,
                CurrentLocation = Home,
                OrderId = "",
                Status = "Ready",
                Id = ""
            };

            var response = await _droneGateway.StartRegistration(newDrone.IpAddress, newDrone.BadgeNumber,
                newDrone.DispatcherUrl, newDrone.HomeLocation);
            if (!response)
                return Problem("either the drone is not ready to be initialized or is already part of a fleet.");
            await _dronesRepository.CreateAsync(newDrone);
            // Todo dispatcher saves handshake record to DB

            await _droneGateway.OKToSendStatus(newDrone.IpAddress);
            return Ok();
        }

        [HttpPost("add_order")]
        public async Task<IActionResult> AddNewOrder(AddOrderDTO order)
        {
            Console.WriteLine("adding a new order");
            bool didSucceed;
            //Todo, eventually availableDrones needs to come from the db 
            // TODO: bug #4 currently this method just creates a drone and an order, making else logic unreachable
            // var availableDrones = await _dronesRepository.GetAllAvailableDronesAsync();
            var availableDrones = new List<Drone>
            {
                new Drone
                {
                    IpAddress = "172.18.0.1:5001",
                    Id = "1",
                    BadgeNumber = new Guid(),
                    OrderId = "",
                    HomeLocation = Home,
                    CurrentLocation = Home,
                    Status = "ready",
                    DispatcherUrl = "172.18.0.0:4000"
                }
            };
            var newOrder = new Order
            {
                Id = order.Id,
                DeliveryLocation = new GeoLocation
                {
                    Latitude = 39.74273568191456m,
                    Longitude = -105.00771026053671m
                }
            };
            if (availableDrones.Any())
            {
                //Todo, this needs to actually come from the repo one day...
                // var newOrder = await _ordersRepository.GetByIdAsync(orderId);
                didSucceed = await _droneGateway.AssignDelivery(availableDrones.First().IpAddress, newOrder.Id,
                    newOrder.DeliveryLocation);
            }
            else
            {
                _unfilledOrders.Enqueue(newOrder);
                didSucceed = true;
            }

            // TODO: unhappy path
            Console.WriteLine($"DispatcherController.AddNewOrder({order}) - Order Complete"); // Debug
            return Ok(didSucceed);
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
            if (_unfilledOrders.Any())
            {
                var droneIpAddress = (await _dronesRepository.GetByIdAsync(droneId)).IpAddress;
                var nextOrder = _unfilledOrders.Dequeue();
                var didSucceed =
                    await _droneGateway.AssignDelivery(droneIpAddress, nextOrder.Id, nextOrder.DeliveryLocation);
            }
            else
            {
                // TODO: Unhappy Path
                Console.WriteLine("Unhappy Path 😓");
            }
            return Ok();
        }

        #endregion endpoints

        //Todo need to put to db... why you not working??
        [HttpPost("update_status")]
        public async Task<IActionResult> UpdateStatus(UpdateStatusDto dto)
        {
            Console.WriteLine($"putting:\n{dto}");
            return Ok();
        }
    }
}