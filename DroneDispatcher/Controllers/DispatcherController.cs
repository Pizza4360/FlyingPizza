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
        private readonly List<IDroneGateway> _drones;
        private readonly Queue<Order> _unfilledOrders;

        public DispatcherController(IDronesRepository droneRepository, IOrdersRepository ordersRepository)
        {
            _dronesRepository = droneRepository;
            _ordersRepository = ordersRepository;
            _drones = new List<IDroneGateway>(); // TODO: get drones from the repository and make actual gateway objects
            _unfilledOrders = new Queue<Order>();
        }

        #region endpoints
        [HttpPost("/register")]
        public async Task<IActionResult> RegisterNewDrone(InitializeDroneRegistrationDto droneInfo)
        {
            var newDrone = new Drone
            {
                BadgeNumber = droneInfo.BadgeNumber,
                IpAddress = droneInfo.IpAddress
            };

            await _dronesRepository.CreateAsync(newDrone);

            // TODO: Register drone w/ dispatcher by doing the following:
                // Todo: wait for OK message back from drone
                // Todo: dispatcher saves handshake record to DB
			    // Todo: dispatcher sends OK back to drone so that drone can stop waiting and start updating status
            return Ok();
        }

        [HttpPost("/add_order")]
        public async Task<IActionResult> AddNewOrder(Order newOrder)
        {
            var didSucceed = false;
            var availableDrones = _drones.Where(drone => drone.GetDroneInfo().Status == Constants.DroneStatus.READY);
            if (availableDrones.Any())
            {
                didSucceed = await availableDrones.First().AssignDeilvery(newOrder.Id, newOrder.DeliveryLocation);
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
                var nextOrder = _unfilledOrders.Dequeue();
                var didSucceed = await _drones
                    .Where(drone => drone.GetDroneInfo().Id == droneId)
                    .First()
                    .AssignDeilvery(nextOrder.Id, nextOrder.DeliveryLocation);

                // TODO: Unhappy Path
            }

            return Ok();
        }
        #endregion endpoints


    }
}
