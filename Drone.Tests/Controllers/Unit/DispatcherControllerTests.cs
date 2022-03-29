using System.Threading.Tasks;
using Domain.DTO.DroneCommunicationDto.DroneToDispatcher;
using Domain.Implementation.Gateways;
using Domain.Interfaces.Gateways;
using DomainImplementation.Repositories;
using DroneDispatcher.Controllers;
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Moq;

namespace Drone.Tests
{
    
 
    public class DispatcherControllerTests
    {

        [Fact]
        public async Task disptacher_controller_should_return_ok()
        {
            var mockedDatabase = new Mock<IMongoDatabase>().Object;
            var mockedDroneGateway = new Mock<IDroneGateway>().Object;
            var controller = new DispatcherController(new DronesRepository(mockedDatabase, "bogus"), mockedDroneGateway);
            var result =  await controller.UpdateStatus(new UpdateStatusDto());
            var expected = new OkResult();
            result.Should().BeEquivalentTo(expected);

        }
    }
}