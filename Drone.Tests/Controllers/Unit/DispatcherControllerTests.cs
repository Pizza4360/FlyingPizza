using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Domain;
using Domain.DTO.DroneCommunicationDto.DispatcherToDrone;
using Domain.DTO.DroneCommunicationDto.DroneToDispatcher;
using Domain.DTO.FrontEndDispatchCommunication.FrontEndToDispatcher;
using Domain.Entities;
using Domain.Interfaces.Gateways;
using Domain.Interfaces.Repositories;
using DroneDispatcher.Controllers;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Drone.Tests.Controllers.Unit
{
    public class DispatcherControllerTests
    {
        private const string TestDeliveryAddress = "yo mama";
        private const string TestOrderNumber = "123";
        private const string TestCustomerName = "bobby";
        private const string TestOrderId = "some stuff";
        private const string InvalidTestIp = "test_ip";
        private const string ValidTestIp = "172.18.0.0";
        private const string TestDispatcherUrl = "http://" + ValidTestIp + ":4000";
        
        private static readonly Guid TestGuid = new();
        private static readonly DateTimeOffset TestTimeDelivered = DateTimeOffset.UtcNow;
        private static readonly GeoLocation TestDeliveryLocation = new()
        {
            Latitude = 39.74362771992734m, Longitude = -105.00549345883957m
        };
        private static readonly GeoLocation TestHomeLocation = new()
        {
            Latitude = 39.74364421910773m,
            Longitude = -105.00561147600774m
        };
        private static readonly Order TestOrder = new()
        {
            DeliveryAddress = TestDeliveryAddress,
            TimeDelivered = TestTimeDelivered,
            Id = TestOrderNumber,
            TimeOrdered = TestTimeDelivered,
            DeliveryLocation = TestDeliveryLocation,
            CustomerName = TestCustomerName
        };
        private static readonly DroneRegistrationInfo BadDroneInfo = new()
        {
            BadgeNumber = TestGuid,
            IpAddress = InvalidTestIp
        };

        private static readonly AddOrderDTO AddOrderDto = new()
        {
            DeliveryLocaion = TestDeliveryLocation,
            Id = TestOrderId
        };
        private static readonly DroneRegistrationInfo DroneRegistrationInfo = new() {
            BadgeNumber = TestGuid,
            IpAddress = ValidTestIp
        };
        private static readonly Domain.Entities.Drone TestDrone = new() {
            IpAddress = ValidTestIp,
            Destination = TestDeliveryLocation,
            BadgeNumber = TestGuid,
            CurrentLocation = TestHomeLocation,
            Status = Constants.DroneStatus.READY,
            OrderId = TestOrderId,
            DispatcherUrl = TestDispatcherUrl,
            Id = "TestGuid",
            HomeLocation = TestHomeLocation
        };
        private static string DroneShouldCreateAsync = "DroneShouldCreateAsync";
        private static string DroneShouldGetByIdAsync = "DroneShouldGetByIdAsync2";
        private static string DroneShouldGetAllAvailableDronesAsync = "DroneShouldGetAllAvailableDronesAsync";
        private static string OrderShouldGetByIdAsync = "OrderShouldGetByIdAsync";
        private static string OrderShouldUpdate = "OrderShouldUpdate";
        private static string GatewayShouldStartRegistration = "GatewayShouldStartRegistration";
        private static string GatewayShouldAssignDelivery2 = "GatewayShouldAssignDelivery2";
        
        private static void SetUpAll(IEnumerable<string> toggles, out DispatcherController controller, out Mock<IOrdersRepository> orders, out Mock<IDroneGateway> gateway)
        {
            var drones = new Mock<IDronesRepository>();
            orders = new Mock<IOrdersRepository>();
            gateway = new Mock<IDroneGateway>();
            var enumerable = toggles as string[] ?? toggles.ToArray();
            if(enumerable.Contains(DroneShouldCreateAsync)) drones.Setup(x => x.CreateAsync(It.IsAny<Domain.Entities.Drone>())).Returns<Domain.Entities.Drone>(Task.FromResult);
            if(enumerable.Contains(DroneShouldGetByIdAsync)) drones.Setup(x => x.GetByIdAsync(It.IsAny<string>())).Returns(Task.FromResult(new Domain.Entities.Drone()));
            if(enumerable.Contains(DroneShouldGetAllAvailableDronesAsync))drones.Setup(x => x.GetAllAvailableDronesAsync()).ReturnsAsync(new List<Domain.Entities.Drone>{TestDrone}.AsEnumerable());
            if(enumerable.Contains(OrderShouldGetByIdAsync))orders.Setup(x => x.GetByIdAsync(It.IsAny<string>())).ReturnsAsync(TestOrder).Verifiable();
            if(enumerable.Contains(OrderShouldUpdate))orders.Setup(x => x.Update(TestOrder)).ReturnsAsync(TestOrder).Verifiable();
            if(enumerable.Contains(GatewayShouldStartRegistration))gateway.Setup(x => x.StartRegistration(It.IsAny<string>(), It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<GeoLocation>())).ReturnsAsync(true).Verifiable();
            if(enumerable.Contains(GatewayShouldAssignDelivery2))gateway.Setup(x => x.AssignDelivery(It.IsAny<string>(),It.IsAny<string>(), It.IsAny<GeoLocation>())).Verifiable();
            controller = new DispatcherController(drones.Object, orders.Object, gateway.Object);
        }

        [Fact]
        public async Task DispatcherControllerShouldReturnOk()
        {
            SetUpAll(new List<string>(), out var controller, out _, out _);
            var result = await controller.UpdateStatus(new UpdateStatusDto());
            var expected = new OkResult();
            result.Should().BeEquivalentTo(expected);
        }
        
        // RegisterNewDrone
        [Fact]
        public async Task DispatcherControllerRegisterShouldSendProperDataToGateway()
        {
            SetUpAll(new List<string>{GatewayShouldStartRegistration, DroneShouldCreateAsync}, out var controller, out _, out var gateway);
            await controller.RegisterNewDrone(DroneRegistrationInfo);
            gateway.Verify();
        }

        [Fact]// InvalidRegistrationNotOk
        public async Task DispatcherControllerShouldReturnProblemOnIncorrectDroneInfo()
        {
            SetUpAll(new List<string>(), out var controller, out _, out _);
            var result = await controller.RegisterNewDrone(BadDroneInfo);
            // Problem object result used in register drone that is invalid for now, will change with black box changes.
            result.Should().NotBeNull();
            result.Should().BeOfType<ObjectResult>();
        }

         [Fact]// ValidRegistrationOk
        public async Task DispatcherShouldReturnOkOnValidDroneInfo()
        {
            SetUpAll(new List<string>{DroneShouldCreateAsync, GatewayShouldStartRegistration}, out var controller, out _, out _);
            var result = await controller.RegisterNewDrone(DroneRegistrationInfo);
            var expected = new OkResult();
            result.Should().BeEquivalentTo(expected);
        }

        
        // OrderAssignedIfDroneAvailable
        [Fact]
        public async Task AddNewOrderShouldAssignAnOrderIfAvailable()
        {
            SetUpAll(new List<string>{DroneShouldGetAllAvailableDronesAsync, DroneShouldGetByIdAsync, OrderShouldGetByIdAsync, GatewayShouldAssignDelivery2}, out var controller, out _, out var gateway);
            await controller.AddNewOrder(AddOrderDto);
            gateway.Verify(x => x.AssignDelivery( It.IsAny<string>(), It.IsAny<string>(), It.IsAny<GeoLocation>()));
        }
        
        [Fact]// AddNewOrderOk
        public async Task AddNewOrderShouldReturnOk()
        {
            SetUpAll(new List<string>{GatewayShouldAssignDelivery2}, out var controller, out _, out _);
            var result = await controller.AddNewOrder(AddOrderDto);
            var expected = new OkResult();
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expected);
        }
        
        // CompleteDeliveryOk
        [Fact]
        public async Task CompleteDeliveryShouldReturnOk()
        {
            SetUpAll(new List<string>{OrderShouldGetByIdAsync, OrderShouldUpdate, GatewayShouldAssignDelivery2}, out var controller, out var orders, out _);
            // calling method
            var result = await controller.CompleteDelivery(TestOrderNumber);
            var expected = new OkResult();
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expected);
            orders.Verify();
        }
        

        [Fact] // CompleteDeliveryUpdatesOrders
        public async Task CompleteDeliveryShouldUpdateOrders()
        {
            SetUpAll(new List<string>{OrderShouldGetByIdAsync, OrderShouldUpdate}, out var controller, out var orders, out _);
            await controller.CompleteDelivery(TestOrderId);
            orders.Verify();
        }

        [Fact]
        public async Task DroneIsReadyForOrdersShouldAlwaysReturnOk()
        {
            SetUpAll(new List<string>(), out var controller, out _, out _);
            var result = await controller.DroneIsReadyForOrder($"{TestGuid}");
            var expected = new OkResult();
            result.Should().NotBeNull();
            result.Should().BeEquivalentTo(expected);
        }
        
        [Fact] //ReadyDroneGetsCorrectOrder
        public async Task DroneIsReadyForOrdersShouldAssignTheCorrectOrder()
        {
            SetUpAll(new List<string>{OrderShouldGetByIdAsync, GatewayShouldAssignDelivery2, DroneShouldGetByIdAsync}, out var controller, out _, out var gateway);
            await controller.AddNewOrder(AddOrderDto);
            // calling method
            await controller.DroneIsReadyForOrder($"{TestGuid}");
        
            // Checking assign has right parameters
            gateway.VerifyAll();
        }
        
        [Fact] // 
        public async Task DroneIsReadyForOrdersShouldGetAsyncCorrectly()
        {
            SetUpAll(new List<string>{OrderShouldGetByIdAsync, DroneShouldGetByIdAsync, DroneShouldGetByIdAsync}, out var controller, out _, out var gateway);
            await controller.AddNewOrder(AddOrderDto);
            // calling method
            await controller.DroneIsReadyForOrder($"{TestGuid}");
            // Checking get async has right parameters
            gateway.Verify();
        }
    }
}