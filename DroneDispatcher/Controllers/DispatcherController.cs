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

        public DispatcherController(
            IDronesRepository droneRepository,
            IOrdersRepository ordersRepository,
            IDroneGateway droneGateway)
        {
            _dronesRepository = droneRepository;
            _ordersRepository = ordersRepository;
            _droneGateway = droneGateway;
            _unfilledOrders = new Queue<Order>();
        }

        #region endpoints
        [HttpPost("/register")]
        public async Task<IActionResult> RegisterNewDrone(InitializeDroneRegistration droneInfo)
        {
            var newDrone = new Drone
            {
                BadgeNumber = droneInfo.BadgeNumber,
                IpAddress = droneInfo.IpAddress
            };


            // Register drone w/ dispatcher by doing the following:
            // wait for OK message back from drone
                var response = await _droneGateway.CompleteRegistration(newDrone.IpAddress, newDrone.Id,
                    newDrone.DispatcherUrl, newDrone.HomeLocation);
                // Todo: save drone to database
                // dispatcher saves handshake record to DB
                await _dronesRepository.CreateAsync(newDrone);
                // dispatcher sends OK back to drone so that drone can stop waiting and start updating status
                await _droneGateway.OKToSendStatus(newDrone.IpAddress);


                return Ok();
        }

        [HttpPost("/add_order")]
        public async Task<IActionResult> AddNewOrder(Order newOrder)
        {
            var didSucceed = false;
            var availableDrones = await _dronesRepository.GetAllAvailableDronesAsync();
            if (availableDrones.Any())
            {
                didSucceed = await _droneGateway.AssignDeilvery(availableDrones.First().IpAddress, newOrder.Id, newOrder.DeliveryLocation);
            }
            else
            {
                _unfilledOrders.Enqueue(newOrder);
                didSucceed = true;
            }

            // TODO: unhappy path

            return Ok();
        }

        [HttpPost("/complete_delivery")]
        public async Task<IActionResult> CompleteDelivery(string orderNumber)
        {
            var order = await _ordersRepository.GetByIdAsync(orderNumber);
            order.TimeDelivered = DateTimeOffset.UtcNow;
            await _ordersRepository.Update(order);

            return Ok();
        }

        [HttpPost("/ready_for_order")]
        public async Task<IActionResult> DroneIsReadyForOrder(string droneId)
        {
            if (_unfilledOrders.Count() > 0)
            {
                var droneIpAddress = (await _dronesRepository.GetByIdAsync(droneId)).IpAddress;
                var nextOrder = _unfilledOrders.Dequeue();
                var didSucceed = await _droneGateway.AssignDeilvery(droneIpAddress, nextOrder.Id, nextOrder.DeliveryLocation);

                // TODO: Unhappy Path
            }

            return Ok();
        }
        #endregion endpoints


    }
}
