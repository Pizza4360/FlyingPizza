using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain.DTO.DispatcherToDrone;
using Domain.DTO.DroneToDispatcher;
using Domain.DTO.FrontEndToDispatcher;
using Domain.Entities;
using Domain.Interfaces.Gateways;
using Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Dispatcher.Controllers
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

        public DispatcherController(
            IDronesRepository droneRepository,
            IOrdersRepository ordersRepository,
            IDroneGateway droneGateway
        )
        {
            _dronesRepository = droneRepository;
            _ordersRepository = ordersRepository;
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
        public async Task<IActionResult> RegisterNewDrone(PostDroneRegistrationDto postDroneDto)
        {
            Console.WriteLine("We are gonna register some shit!!");
            // Todo make a new guid and make sure it is different from all other drones
            var newDrone = new DroneRecord
            {
                BadgeNumber = postDroneDto.BadgeNumber,
                IpAddress = postDroneDto.IpAddress,
                HomeLocation = Home,
                DispatcherUrl = "http://172.18.0.0:4000",
                Destination = Home,
                CurrentLocation = Home,
                OrderId = "",
                State = DroneState.Ready,
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
        public async Task<IActionResult> AddNewOrder(PostAddOrderDto order)
        {
            Console.WriteLine("adding a new order");
            bool didSucceed;
             var availableDrones = await _dronesRepository.GetAllAvailableDronesAsync();
            
            var newOrder = await _ordersRepository.GetByIdAsync(order.Id);
            if (availableDrones.Any())
            {
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
        public async Task<IActionResult> UpdateStatus(PutStatusDto dto)
        {
            Console.WriteLine($"putting:\n{dto}");
            await _dronesRepository.Update(new DroneRecord
            {
                CurrentLocation = dto.Location,
                State = dto.State,
                BadgeNumber = new Guid(dto.BadgeNumber)
            });
            return Ok();
        }
    }
}