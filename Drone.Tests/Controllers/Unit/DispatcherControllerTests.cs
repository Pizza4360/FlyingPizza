using System;
using System.Collections.Generic;
using System.Linq;
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
        private const string TestDeliveryAddress = "yo mama";
        private const string TestCustomerName = "bobby";
        private const string TestOrderId = "some stuff";
        private const string TestDroneIp = "test_ip";
        private const string TestDispatcherUrl = "http://172.18.0.0:4000";
        
        private static Guid TestGuid = new();
        private static DateTimeOffset TestTimeDelivered = DateTimeOffset.UtcNow;
        private static GeoLocation TestDeliveryLocation = new()
        {
            Latitude = 39.74362771992734m, Longitude = -105.00549345883957m
        };
        private static GeoLocation TestHomeLocation = new()
        {
            Latitude = 39.74364421910773m,
            Longitude = -105.00561147600774m
        };
        private static Order TestFakeOrder = new()
        {
            DeliveryAddress = TestDeliveryAddress,
            TimeDelivered = TestTimeDelivered,
            Id = TestOrderId,
            TimeOrdered = TestTimeDelivered,
            DeliveryLocation = TestDeliveryLocation,
            CustomerName = TestCustomerName
        };

        private static AddOrderDTO _addOrderDto = new()
        {
            DeliveryLocaion = TestDeliveryLocation,
            Id = TestOrderId
        };
        private static DroneRegistrationInfo DroneRegistrationInfo = new() {
            BadgeNumber = TestGuid,
            IpAddress = TestDroneIp
        };
        private static Domain.Entities.Drone TestDrone = new() {
            IpAddress = TestDroneIp,
            Destination = TestDeliveryLocation,
            BadgeNumber = TestGuid,
            CurrentLocation = TestHomeLocation,
            Status = Domain.Constants.DroneStatus.READY,
            OrderId = TestOrderId,
            DispatcherUrl = TestDispatcherUrl,
            Id = "TestGuid",
            HomeLocation = TestHomeLocation
        };
        
        private IOrdersRepository _mockOrdersRepo;
        private IMongoDatabase _mockDatabase;
        private IDroneGateway _mockDroneGateway;
        private IDroneGateway _mockDroneGateway2;
        private DispatcherController _controller;
        private static Mock<IDroneGateway> _droneGatewaySetup = new();
        private static Mock<IDroneGateway> _droneGatewaySetup2 = new();
        private static Mock<IDronesRepository> _droneRepoSetup = new();
        private static Mock<IOrdersRepository> _orderRepoSetup = new();

        private IDronesRepository _mockDroneRepository;

        public DispatcherControllerTests()
        {
            // Set up mocked drone gateway
            _droneGatewaySetup.Setup(x => x.StartRegistration(
                TestDroneIp,
                TestGuid, 
                TestDispatcherUrl,
                TestHomeLocation)
            ).Returns(Task.FromResult(true)).Verifiable();
            _droneGatewaySetup.Setup(x => x.AssignDelivery(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<GeoLocation>()))
                .Returns(Task.FromResult(true)).Verifiable();
            _mockDroneGateway = _droneGatewaySetup.Object;
            
            // Set up mocked drone repository
            _droneRepoSetup = new Mock<IDronesRepository>();
            _droneRepoSetup.Setup(x => x.CreateAsync(It.IsAny<Domain.Entities.Drone>())).Returns<Domain.Entities.Drone>(Task.FromResult);
            _droneRepoSetup.Setup(_ => _.GetAllAvailableDronesAsync()).Returns(Task.FromResult(
                new List<Domain.Entities.Drone>(1) { TestDrone }.AsEnumerable()));
            _droneRepoSetup.Setup(x => x.GetByIdAsync(It.IsAny<string>())) .Returns(Task.FromResult(new Domain.Entities.Drone()));
            _mockDroneRepository = _droneRepoSetup.Object;
            
            // Second version forcing mock of gateway to say drone is valid
            _droneGatewaySetup2 = new Mock<IDroneGateway>();
            _droneGatewaySetup2
                .Setup(x => x.StartRegistration(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<string>(),
                    It.IsAny<GeoLocation>())).Returns(Task.FromResult(true));
            _mockDroneGateway2 = _droneGatewaySetup2.Object;
            
            // Set up mocked orders repository
            _orderRepoSetup.Setup(x => x.GetByIdAsync("some stuff")).Returns(Task.FromResult(TestFakeOrder));
            _mockOrdersRepo = _orderRepoSetup.Object;
            _mockOrdersRepo = new Mock<IOrdersRepository>().Object;
           
            // Set up mocked database
            _mockDatabase = new Mock<IMongoDatabase>().Object;
            
            // Set up mocked controller
            _controller = new DispatcherController(_mockDroneRepository, _mockOrdersRepo, _mockDroneGateway );
            
        }
        
        // Update Status
        [Fact]
        public async Task DispatcherControllerShouldReturnOk()
        {
            var result = await _controller.UpdateStatus(new UpdateStatusDto());
            var expected = new OkResult();
            result.Should().BeEquivalentTo(expected);
        }
        

        // RegisterNewDrone
        [Fact]
        public async Task DispatcherControllerRegisterShouldSendProperDataToGateway()
        {
            await _controller.RegisterNewDrone(DroneRegistrationInfo);
            _droneGatewaySetup.VerifyAll();
        }

        [Fact]
        public async Task DispatcherControllerShouldReturnProblemOnIncorrectDroneInfo()
        {
            var result = await _controller.RegisterNewDrone(DroneRegistrationInfo);
            // Problem object result used in register drone that is invalid for now, will change with black box changes.
            result.Should().NotBeNull();
            result.Should().BeOfType<ObjectResult>();
        }

        [Fact]
        public async Task DispatcherShouldReturnOkOnValidDroneInfo()
        {
            var controller = new DispatcherController(_mockDroneRepository, _mockOrdersRepo, _mockDroneGateway2);
            var result = await controller.RegisterNewDrone(DroneRegistrationInfo);
            var expected = new OkResult();
            result.Should().BeEquivalentTo(expected);
        }


        // addNewOrder
        [Fact]
        public async Task AddNewOrderShouldAssignAnOrderIfAvailable()
        {
            // calling method
            await _controller.AddNewOrder(_addOrderDto);
            _droneGatewaySetup.VerifyAll();
        }

        [Fact]
        public async Task AddNewOrderShouldReturnOk()
        {
            var result = await _controller.AddNewOrder(_addOrderDto);
            var expected = new OkResult();
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expected);
        }

        // CompleteDelivery
        [Fact]
        public async Task CompleteDeliveryShouldReturnOk()
        {
            var result = await _controller.CompleteDelivery(TestOrderId);
            var expected = new OkResult();
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expected);
            _orderRepoSetup.VerifyAll();
        }

        [Fact]
        public async Task CompleteDeliveryShouldUpdateOrders()
        {
            await _controller.CompleteDelivery(TestOrderId);
            _orderRepoSetup.VerifyAll();
        }

        // DroneIsReadyForOrders
        [Fact]
        public async Task DroneIsReadyForOrdersShouldAlwaysReturnOk()
        {
            var result = await _controller.DroneIsReadyForOrder($"{TestGuid}");
            var expected = new OkResult();
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expected);
        }

        [Fact]
        public async Task DroneIsReadyForOrdersShouldAssignTheCorrectOrder()
        {
            await _controller.AddNewOrder(_addOrderDto);
            // calling method
            await _controller.DroneIsReadyForOrder($"{TestGuid}");

            // Checking assign has right parameters
            _droneGatewaySetup.VerifyAll();
        }

        [Fact]
        public async Task DroneIsReadyForOrdersShouldGetAsyncCorrectly()
        {
            // adding wanted order to orders
            await _controller.AddNewOrder(_addOrderDto);
            // calling method
            await _controller.DroneIsReadyForOrder($"{TestGuid}");
            // Checking getasync has right parameters
            _droneGatewaySetup.VerifyAll();
        }
    }
}