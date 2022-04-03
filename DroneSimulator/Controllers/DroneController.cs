using System;
using Domain.DTO.DroneCommunicationDto.DispatcherToDrone;
using Domain.Entities;
using Domain.Interfaces.Gateways;
using Domain.Interfaces.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using MongoDB.Driver.Core.Events;

namespace DroneSimulator.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class DroneController : ControllerBase
    {
        private readonly IDronesRepository _dronesRepository;
        private readonly IDispatcherGateway _dispatcherGateway;
        private Drone _drone;

        public DroneController(IDronesRepository dronesRepository, IDispatcherGateway dispatcherGateway)
        {
            _dronesRepository = dronesRepository;
            _dispatcherGateway = dispatcherGateway;
            _drone = new Drone("123", new GeoLocation
            {
                Latitude = 39.74364421910773m,
                Longitude = -105.00561147600774m
            }, dispatcherGateway); // TODO: initialize this from the drones repository, based on a drone id from environment variables
            Console.WriteLine(_drone);
        }

        [HttpPost("assigndelivery")]
        public async Task<IActionResult> AssignDelivery(DeliverOrderDto order)
        {
            Console.WriteLine($"Assigned order `{order.OrderId}`");
            _drone.DeliverOrder(order.OrderLocation);
            Console.WriteLine($"Done with order `{order.OrderId}`");
            return Ok();
        }

        [HttpPost("initregistration")]
        public async Task<IActionResult> InitializeRegistration()
        {
            Console.WriteLine($"Initializing{_drone}");
            // Todo, add logic to verify legitimacy of adding a drone.
            return Ok($"Initializing{_drone}");
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
