using System;
using Domain.DTO.DroneCommunicationDto.DispatcherToDrone;
using Domain.Entities;
using Domain.Interfaces.Gateways;
using Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Domain.Implementation.Gateways;

namespace DroneSimulator.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DroneController : ControllerBase
    {
        private readonly IDronesRepository _dronesRepository;
        private readonly DispatcherGateway _dispatcherGateway;
        private Drone _drone;

        public DroneController(IDronesRepository dronesRepository, IDispatcherGateway dispatcherGateway)
        {
            _dronesRepository = dronesRepository;
            _drone = new Drone("123", new GeoLocation
            {
                Latitude = 39.74364421910773m,
                Longitude = -105.00561147600774m
            }, dispatcherGateway,
                5, "1", "something"); // TODO: initialize this from the drones repository, based on a drone id from environment variables
            Console.WriteLine(_drone);
        }

        [HttpPost("assigndelivery")]
        public async Task<IActionResult> AssignDelivery(DeliverOrderDto order)
        {
            _drone.DeliverOrder(order.OrderLocation);
            return Ok();
        }

        [HttpPost("initregistration")]
        public async Task<IActionResult> InitializeRegistration()
        {
            Console.WriteLine($"Initializing{_drone}");
            // Todo, add logic to verify legitimacy of adding a drone.
            return Ok("Drone successfully initialized");
        }

        [HttpPost("completeregistration")]
        public async Task<IActionResult> CompleteRegistration()
        {
            return Ok();
        }

        public void changeDrone(Drone drone)
        {
            _drone = drone;
        }
    }
}
