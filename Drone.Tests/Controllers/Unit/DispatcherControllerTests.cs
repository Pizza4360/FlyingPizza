using System;
using System.Threading.Tasks;
using Domain.DTO.DroneCommunicationDto.DispatcherToDrone;
using Domain.DTO.DroneCommunicationDto.DroneToDispatcher;
using Domain.DTO.FrontEndDispatchCommunication.FrontEndToDispatcher;
using Domain.Entities;
using Domain.Interfaces.Gateways;
using Domain.Interfaces.Repositories;
using DomainImplementation.Repositories;
using DroneDispatcher.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Moq;
using Xunit;

namespace Drone.Tests.Controllers.Unit
{


    public class DispatcherControllerTests
    {


        // Update Status
        [Fact]
        public async Task dispatcher_controller_should_return_ok()
        {
            var mockedDatabase = new Mock<IMongoDatabase>().Object;
            var mockedDroneGateway = new Mock<IDroneGateway>().Object;
            var controller =
                new DispatcherController(new DronesRepository(mockedDatabase, "bogus"), mockedDroneGateway);
            var result = await controller.UpdateStatus(new UpdateStatusDto());
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
            mockedDroneGatewaySetup.Setup(x => x.StartRegistration("test_ip", testGuid, "http://172.18.0.0:4000",
                new GeoLocation
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
            var mockedDroneRepositorySetup = new Mock<IDronesRepository>();
            mockedDroneRepositorySetup.Setup(x => x.CreateAsync(It.IsAny<Domain.Entities.Drone>())).Returns<Domain.Entities.Drone>(x =>Task.FromResult(x));
            var mockedDroneRepository = mockedDroneRepositorySetup.Object;
            var controller =
                new DispatcherController(mockedDroneRepository, mockedDroneGateway);
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
            var controller =
                new DispatcherController(new DronesRepository(mockedDatabase, "bogus"), mockedDroneGateway);
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
            mockedDronesRepositorySetup.Setup(x => x.CreateAsync(It.IsAny<Domain.Entities.Drone>()))
                .Returns<Domain.Entities.Drone>((x => Task.FromResult(x)));
            var mockedDronesRepository = mockedDronesRepositorySetup.Object;
            // Forcing mock of gateway to say drone is valid
            var mockedDroneGatewaySetup = new Mock<IDroneGateway>();
            mockedDroneGatewaySetup
                .Setup(x => x.StartRegistration(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<string>(),
                    It.IsAny<GeoLocation>())).Returns(Task.FromResult(true));
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


        // addNewOrder
        [Fact]
        public async Task add_new_order_should_assign_an_order_if_available()
        {
            var mockedDronesRepository = new Mock<IDronesRepository>().Object;
            var mockedDroneGatewaySetup = new Mock<IDroneGateway>();
            mockedDroneGatewaySetup
                .Setup(x => x.AssignDelivery(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<GeoLocation>()))
                .Returns(Task.FromResult(true)).Verifiable();
            var mockedDroneGateway = mockedDroneGatewaySetup.Object;
            var testGuid = new Guid();
            var testInfo = new DroneRegistrationInfo
            {
                BadgeNumber = testGuid,
                IpAddress = "test_ip"
            };
            var controller = new DispatcherController(mockedDronesRepository, mockedDroneGateway);
            var testOrderDto = new AddOrderDTO();

            // adding an available drone
            await controller.RegisterNewDrone(testInfo);
            // calling method
            await controller.AddNewOrder(testOrderDto);

            mockedDroneGatewaySetup.VerifyAll();
        }

        [Fact]
        public async Task add_new_order_should_return_ok()
        {
            var mockedDronesRepository = new Mock<IDronesRepository>().Object;
            var mockedDroneGatewaySetup = new Mock<IDroneGateway>();
            mockedDroneGatewaySetup
                .Setup(x => x.AssignDelivery(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<GeoLocation>()))
                .Returns(Task.FromResult(true)).Verifiable();
            var mockedDroneGateway = mockedDroneGatewaySetup.Object;
            var testGuid = new Guid();
            var testInfo = new DroneRegistrationInfo
            {
                BadgeNumber = testGuid,
                IpAddress = "test_ip"
            };
            var controller = new DispatcherController(mockedDronesRepository, mockedDroneGateway);
            var testOrderDto = new AddOrderDTO();
            // calling method
            var result = await controller.AddNewOrder(testOrderDto);

            var expected = new OkResult();
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expected);
        }

        // CompleteDelivery
        [Fact]
        public async Task complete_delivery_should_return_ok()
        {
            var mockedDronesRepository = new Mock<IDronesRepository>().Object;
            var mockedDroneGateway = new Mock<IDroneGateway>().Object;
            var mockedOrdersRepositorySetup = new Mock<IOrdersRepository>();
            var testOrderNumber = "35";
            var testOrder = new Order
            {
                Id = testOrderNumber,
                CustomerName = "testee",
                DeliveryLocation = new GeoLocation
                {
                    Latitude = 69,
                    Longitude = 69
                },
                TimeOrdered = DateTimeOffset.UtcNow
                // TimeDelivered assumed null by business logic
            };
 
            // Ensuring orders repo is updated
            mockedOrdersRepositorySetup.Setup(x => x.GetByIdAsync(testOrderNumber)).Returns(Task.FromResult(testOrder)).Verifiable();
            mockedOrdersRepositorySetup.Setup(x => x.Update(testOrder)).Returns(Task.FromResult(testOrder)).Verifiable();
            var mockedOrdersRepository = mockedOrdersRepositorySetup.Object;
            // TODO: BUG # 2 currently fails since Dev hasn't added order repo to dispatcher
            
            var controller = new DispatcherController(mockedDronesRepository, mockedDroneGateway);
            // calling method
            var result = await controller.CompleteDelivery(testOrderNumber);

            var expected = new OkResult();
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expected);
            mockedOrdersRepositorySetup.VerifyAll();
        }
        
        [Fact]
        public async Task complete_delivery_should_update_orders()
        {
            var mockedDronesRepository = new Mock<IDronesRepository>().Object;
            var mockedDroneGateway = new Mock<IDroneGateway>().Object;
            var mockedOrdersRepositorySetup = new Mock<IOrdersRepository>();
            var testOrderNumber = "35";
            var testOrder = new Order
            {
                Id = testOrderNumber,
                CustomerName = "testee",
                DeliveryLocation = new GeoLocation
                {
                    Latitude = 69,
                    Longitude = 69
                },
                TimeOrdered = DateTimeOffset.UtcNow
                // TimeDelivered assumed null by business logic
            };
 
            // Ensuring orders repo is updated
            mockedOrdersRepositorySetup.Setup(x => x.GetByIdAsync(testOrderNumber)).Returns(Task.FromResult(testOrder)).Verifiable();
            mockedOrdersRepositorySetup.Setup(x => x.Update(testOrder)).Returns(Task.FromResult(testOrder)).Verifiable();
            var mockedOrdersRepository = mockedOrdersRepositorySetup.Object;
            // TODO:BUG #2 currently fails since Dev hasn't added order repo to dispatcher
            
            var controller = new DispatcherController(mockedDronesRepository, mockedDroneGateway);
            // calling method
            await controller.CompleteDelivery(testOrderNumber);

            mockedOrdersRepositorySetup.VerifyAll();
        }

        // DroneIsReadyForOrders
        [Fact]
        public async Task drone_is_ready_for_orders_should_always_return_ok()
        {
            
            var mockedDronesRepository = new Mock<IDronesRepository>().Object;
            var mockedDroneGateway = new Mock<IDroneGateway>().Object;
            var testGuidString = "invalid in every way";
            var controller = new DispatcherController(mockedDronesRepository, mockedDroneGateway);
            // calling method
            var result = await controller.DroneIsReadyForOrder(testGuidString);

            var expected = new OkResult();
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expected);
        }
        
        [Fact]
        public async Task drone_is_ready_for_orders_should_assign_the_correct_order()
        {
            
            var mockedDronesRepository = new Mock<IDronesRepository>().Object;
            var mockedDroneGatewaySetup  = new Mock<IDroneGateway>();
            
            // Loading up with test order
            var testGuidString = "close enough";
            var testLocation = new GeoLocation
            {
                Latitude = 69,
                Longitude = 69
            };
            var testOrderDto = new AddOrderDTO
            {
                DeliveryLocaion = testLocation,
                Id = testGuidString
            };
            mockedDroneGatewaySetup.Setup(x => x.AssignDelivery(It.IsAny<string>(),testGuidString, testLocation)).Verifiable();
            var mockedDroneGateway = mockedDroneGatewaySetup.Object;
            
            var controller = new DispatcherController(mockedDronesRepository, mockedDroneGateway);
            
            
            // adding wanted order to orders
            await controller.AddNewOrder(testOrderDto);
            
            // calling method
            await controller.DroneIsReadyForOrder(testGuidString);

            // Checking assign has right parameters
            mockedDroneGatewaySetup.VerifyAll();
        }
    
        [Fact]
        public async Task drone_is_ready_for_orders_should_get_async_correctly()
        {
            
            var mockedDronesRepositorySetup = new Mock<IDronesRepository>();
            mockedDronesRepositorySetup.Setup(x => x.GetByIdAsync(It.IsAny<string>())).Verifiable();
            var mockedDronesRepository = mockedDronesRepositorySetup.Object;
            var mockedDroneGatewaySetup  = new Mock<IDroneGateway>();
            
            // Loading up with test order
            var testGuidString = "close enough";
            var testLocation = new GeoLocation
            {
                Latitude = 69,
                Longitude = 69
            };
            var testOrderDto = new AddOrderDTO
            {
                DeliveryLocaion = testLocation,
                Id = testGuidString
            };
            var mockedDroneGateway = mockedDroneGatewaySetup.Object;
            
            var controller = new DispatcherController(mockedDronesRepository, mockedDroneGateway);
            
            
            // adding wanted order to orders
            await controller.AddNewOrder(testOrderDto);
            
            // calling method
            await controller.DroneIsReadyForOrder(testGuidString);

            // Checking getasync has right parameters
            mockedDroneGatewaySetup.VerifyAll();
        }
}
}