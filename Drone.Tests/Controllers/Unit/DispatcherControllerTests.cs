using System;
using System.Threading.Tasks;
using Domain.DTO.DroneCommunicationDto.DispatcherToDrone;
using Domain.DTO.DroneCommunicationDto.DroneToDispatcher;
using Domain.Entities;
using Domain.Implementation.Gateways;
using Domain.Interfaces.Gateways;
using Domain.Interfaces.Repositories;
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

        
        // Update Status
        [Fact]
        public async Task dispatcher_controller_should_return_ok()
        {
            var mockedDatabase = new Mock<IMongoDatabase>().Object;
            var mockedDroneGateway = new Mock<IDroneGateway>().Object;
            var controller = new DispatcherController(new DronesRepository(mockedDatabase, "bogus"), mockedDroneGateway);
            var result =  await controller.UpdateStatus(new UpdateStatusDto());
            var expected = new OkResult();
            result.Should().BeEquivalentTo(expected);

        }

        // RegisterNewDrone
        [Fact]
        public async Task dispatcher_controller_register_should_send_proper_data_to_gateway()
        {
            var mockedDatabase = new Mock<IMongoDatabase>().Object;
            var mockedDroneGatewaySetup = new Mock<IDroneGateway>();
            var testGuid = new Guid();
            // Here we set up a verify hook so we can see if the right arguments were used in a startRegistration call
            mockedDroneGatewaySetup.Setup(x => x.StartRegistration("test_ip", testGuid, "http://172.18.0.0:4000", new GeoLocation
                {
                     Latitude = 39.74364421910773m,
                     Longitude = -105.00561147600774m
                 })).Returns(Task.FromResult(true)).Verifiable();

            var mockedDroneGateway = mockedDroneGatewaySetup.Object;
            
            var testInfo = new DroneRegistrationInfo
            {
                BadgeNumber = testGuid,
                IpAddress = "test_ip"
            };
            var controller = new DispatcherController(new DronesRepository(mockedDatabase, "bogus"), mockedDroneGateway);
            await controller.RegisterNewDrone(testInfo);
            mockedDroneGatewaySetup.VerifyAll();
        }

        [Fact]
        public async Task dispatcher_controller_should_return_problem_on_incorrect_drone_info()
        {
            var mockedDatabase = new Mock<IMongoDatabase>().Object;
            var mockedDroneGateway = new Mock<IDroneGateway>().Object;
            var testGuid = new Guid();
            
            var testInfo = new DroneRegistrationInfo
            {
                BadgeNumber = testGuid,
                IpAddress = "test_ip"
            };
            var controller = new DispatcherController(new DronesRepository(mockedDatabase, "bogus"), mockedDroneGateway);
            var result = await controller.RegisterNewDrone(testInfo);
            // Problem object result used in register drone that is invalid for now, will change with black box changes.
            result.Should().NotBeNull();
            result.Should().BeOfType<ObjectResult>();
        }

        [Fact]
        public async Task dispatcher_should_return_ok_on_valid_drone_info()
        {
            var mockedDronesRepositorySetup = new Mock<IDronesRepository>();
            // Forcing mongo mock to accept non-existent drone without connecting to server
            mockedDronesRepositorySetup.Setup(x => x.CreateAsync(It.IsAny<Domain.Entities.Drone>())).Returns<Domain.Entities.Drone>((x => Task.FromResult(x)));
            var mockedDronesRepository = mockedDronesRepositorySetup.Object;
            // Forcing mock of gateway to say drone is valid
            var mockedDroneGatewaySetup = new Mock<IDroneGateway>();
                mockedDroneGatewaySetup.Setup(x => x.StartRegistration(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<GeoLocation>())).Returns(Task.FromResult(true));
                var mockedDroneGateway = mockedDroneGatewaySetup.Object;
            var testGuid = new Guid();
            
            var testInfo = new DroneRegistrationInfo
            {
                BadgeNumber = testGuid,
                IpAddress = "test_ip"
            };
            var controller = new DispatcherController(mockedDronesRepository, mockedDroneGateway);
            var result = await controller.RegisterNewDrone(testInfo);
            var expected = new OkResult();
            result.Should().BeEquivalentTo(expected);
        }
    }
}