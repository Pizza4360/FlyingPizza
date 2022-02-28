using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.DTO.DroneCommunicationDto.DispatcherToDrone;
using Domain.Entities;
using Domain.Interfaces.Gateways;
using Domain.Interfaces.Repositories;
using DroneDispatcher.Controllers;
using Xunit;

namespace Drone.Tests
{
    public class MockOrders : IOrdersRepository
    {
        public List<Order> Orders { get;}

        public MockOrders()
        {
            Orders = new List<Order>();
        }
        
        // Added to easily see mocked order
        public Order FindOrder(string id)
        {
            return Orders.Find(x => x.Id == id);
        }
        public Task<Order> CreateAsync(Order entity)
        {
            throw new NotImplementedException();
        }

        public Task<Order> GetByIdAsync(string id)
        {
            var order = Orders.Find(x => x.Id == id);
            return Task.FromResult(order);
        }

        public Task<IEnumerable<Order>> GetByIdsAsync(IEnumerable<string> ids)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(string id)
        {
            throw new NotImplementedException();
        }

        public Task<Order> Update(Order entity)
        {
            var index = Orders.FindIndex(x => x.Id == entity.Id);
            if (index >= 0)
            {
                Orders.RemoveAt(index);
            }
            Orders.Add(entity);
            // Assuming not patching fields, and instead providing whole entity.
            return Task.FromResult(entity);
        }
    }

    public class MockDroneRepo : IDronesRepository
    {
        public MockDroneRepo()
        {
            Drones = new List<Domain.Entities.Drone>();
        }
        public List<Domain.Entities.Drone> Drones { get; set; }

        public Task<Domain.Entities.Drone> CreateAsync(Domain.Entities.Drone entity)
        {
            
            Drones.Add(entity);
            return Task.FromResult(entity);
        }

        public Task<Domain.Entities.Drone> GetByIdAsync(string id)
        {
            var found = Drones.Find(x => x.Id == id);
            return Task.FromResult(found);
        }

        public Task<IEnumerable<Domain.Entities.Drone>> GetByIdsAsync(IEnumerable<string> ids)
        {
            throw new NotImplementedException();
        }

        public Task<bool> Delete(string id)
        {
            Drones.RemoveAll(x => x.Id == id);
            return Task.FromResult(true);
        }

        public Task<Domain.Entities.Drone> Update(Domain.Entities.Drone entity)
        {
            throw new NotImplementedException();
        }

        public Task<Domain.Entities.Drone> GetDroneOnOrderAsync(string orderNumber)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Domain.Entities.Drone>> GetAllAvailableDronesAsync()
        {
            IEnumerable<Domain.Entities.Drone> resultList = Drones;
            return Task.FromResult(resultList);
        }

        public bool HasDrone(string ip)
        {
            var found = Drones.Find(x => x.IpAddress == ip);
            return found != null && (found.IpAddress == ip);
        }
    }

    public class MockGateway : IDroneGateway
    {
        public Domain.Entities.Drone DroneAtGateway { get; private set; }

        public Task<bool> CompleteRegistration(string droneIpAddress, Guid badgeNumber, string dispatcherUrl, GeoLocation homeLocation)
        {
            DroneAtGateway = new Domain.Entities.Drone
            {
                IpAddress = droneIpAddress,
                BadgeNumber = badgeNumber,
                DispatcherUrl = dispatcherUrl,
                HomeLocation = homeLocation
            };
            return Task.FromResult(true);
        }

        public Task<bool> AssignDelivery(string droneIpAddress, string orderNumber, GeoLocation orderLocation)
        {
            DroneAtGateway.OrderId = orderNumber;
            DroneAtGateway.Destination = orderLocation;
            return Task.FromResult(true);
        }

        public Task<bool> OKToSendStatus(string droneIpAddress)
        {
            return Task.FromResult(true);
        }
    }
    public class DispatcherTests
    {
        // Fixtures for quality of life
        
        
        // Mock objects
        // This format was suggested by lint, its ugly
        private static readonly MockOrders Orders = new();
        private static readonly MockDroneRepo Drones = new();
        private static readonly MockGateway Gateway = new();
        
        // Assert fail
        // private static void AssertFail(string msg)
        // {
        //     Assert.True(false, msg);
        // }
        
        // Tests
        [Fact]
        public void TestRegister()
        {
            var controller = new DispatcherController(Drones, Orders, Gateway, new GeoLocation());
            var droneInfo = new InitializeDroneRegistration
            {
                BadgeNumber = new Guid(),
                IpAddress = "Test IP",
                DispatcherUrl = "TestUrl"
            };
            // Replace other pieces with easily viewable mock objects
            var resp1 = controller.RegisterNewDrone(droneInfo);
            resp1.Wait();
            
            // Checking mocked objects for expected pieces            
            Assert.True(resp1.IsCompletedSuccessfully, "Registration failed to return");
            Assert.True(Drones.HasDrone(droneInfo.IpAddress), "drone failed to register in mocked repo");
        }
        [Fact]
        public void TestAddOrder()
        {
            var controller = new DispatcherController(Drones, Orders, Gateway, new GeoLocation());
            var droneInfo = new InitializeDroneRegistration
            {
                BadgeNumber = new Guid(),
                IpAddress = "Test IP",
                DispatcherUrl = "TestUrl"
            };
            var resp1 = controller.RegisterNewDrone(droneInfo);
            resp1.Wait();
            var testOrder = new Order
            {
                Id = "test",
                CustomerName = "test",
                DeliveryAddress = "test",
                DeliveryLocation = controller.Home,
                TimeOrdered = new DateTimeOffset(),
                TimeDelivered = new DateTimeOffset()
            };
            var resp2 = controller.AddNewOrder(testOrder);
            resp2.Wait();
            Assert.True(resp2.IsCompletedSuccessfully, "Failed to create a new order");
            Assert.Equal(Gateway.DroneAtGateway.OrderId, testOrder.Id);
            Assert.Equal(Gateway.DroneAtGateway.Destination, testOrder.DeliveryLocation);
        }

        [Fact]
        public void TestCompleteDelivery()
        {
            var controller = new DispatcherController(Drones, Orders, Gateway, new GeoLocation());
            var droneInfo = new InitializeDroneRegistration
            {
                BadgeNumber = new Guid(),
                IpAddress = "Test IP",
                DispatcherUrl = "TestUrl"
            };
            var resp1 = controller.RegisterNewDrone(droneInfo);
            resp1.Wait();

            var testOrder = new Order
            {
                Id = "test",
                CustomerName = "test",
                DeliveryAddress = "test",
                DeliveryLocation = controller.Home,
                TimeOrdered = DateTimeOffset.UtcNow,
                TimeDelivered = DateTimeOffset.UtcNow
            };
            var testTime = testOrder.TimeDelivered;
            Orders.Orders.Add(testOrder);
            var resp2 = controller.AddNewOrder(testOrder);
            resp2.Wait();
            var resp3 = controller.CompleteDelivery(testOrder.Id);
            resp3.Wait();
            var dateTimeOffset = Orders.FindOrder(testOrder.Id).TimeDelivered;
            if (dateTimeOffset != null)
                Assert.NotEqual(testTime, dateTimeOffset.Value);
        }

        [Fact]
        public void TestReadyForOrder()
        {
            var controller = new DispatcherController(Drones, Orders, Gateway, new GeoLocation());
            var droneInfo = new InitializeDroneRegistration
            {
                BadgeNumber = new Guid(),
                IpAddress = "Test IP",
                DispatcherUrl = "TestUrl"
            };
            var resp1 = controller.RegisterNewDrone(droneInfo);
            resp1.Wait();

            var testLocation = new GeoLocation
            {
                Latitude = 69,
                Longitude = 69
            };
            var testOrder = new Order
            {
                Id = "test",
                CustomerName = "test",
                DeliveryAddress = "test",
                DeliveryLocation = testLocation,
                TimeOrdered = DateTimeOffset.UtcNow,
                TimeDelivered = DateTimeOffset.UtcNow
            };
            var testDroneId = Drones.Drones[0].Id;
            Orders.Orders.Add(testOrder);
            var resp2 = controller.AddNewOrder(testOrder);
            resp2.Wait();
            var resp3 = controller.DroneIsReadyForOrder(testDroneId);
            resp3.Wait();
            Assert.Equal(Gateway.DroneAtGateway.Destination, testOrder.DeliveryLocation);
        }
    }
}