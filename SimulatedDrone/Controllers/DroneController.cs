using System;
using Domain.Entities;
using Domain.Interfaces.Gateways;
using Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Domain.DTO.DispatcherToDrone;

namespace DroneSimulator.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DroneController : ControllerBase
    {
        private readonly IDronesRepository _dronesRepository;
        private readonly IDispatcherGateway _dispatcherGateway;
        private SimDrone _simDrone;

        public DroneController(IDronesRepository dronesRepository, IDispatcherGateway dispatcherGateway)
        {
            _dronesRepository = dronesRepository;
            _dispatcherGateway = dispatcherGateway;
            _simDrone = new SimDrone("123", new GeoLocation
            {
                Latitude = 39.74364421910773m,
                Longitude = -105.00561147600774m
            }, dispatcherGateway); // TODO: initialize this from the drones repository, based on a drone id from environment variables
            Console.WriteLine(_simDrone);
        }

        [HttpPost("assigndelivery")]
        public async Task<IActionResult> AssignDelivery(PostDeliverOrderDto order)
        {
            Console.WriteLine($"Assigned order `{order.OrderId}`");
            _simDrone.DeliverOrder(order.OrderLocation);
            Console.WriteLine($"Done with order `{order.OrderId}`");
            return Ok();
        }

        [HttpPost("initregistration")]
        public async Task<IActionResult> InitializeRegistration()
        {
            Console.WriteLine($"Initializing{_simDrone}");
            // Todo, add logic to verify legitimacy of adding a drone.
            return Ok($"Initializing{_simDrone}");
        }

        [HttpPost("completeregistration")]
        public async Task<IActionResult> CompleteRegistration()
        {
            return Ok();
        }

        public void changeDrone(SimDrone simDrone)
        {
            _simDrone = simDrone;
        }
    }
}
