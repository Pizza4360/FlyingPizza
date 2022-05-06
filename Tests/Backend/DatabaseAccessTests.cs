using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.RepositoryDefinitions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Tests.Backend;

public class DatabaseAccessTests
{
    [Fact]
    public async Task get_fleet_should_get_list_of_drone_records()
    {
        var mockLogger = new Mock<ILogger<DatabaseAccess.Controllers.DatabaseAccess>>();
        var mockFleet = new Mock<IFleetRepository>();
        var mockOrders = new Mock<IOrdersRepository>();
        var testDb = new List<DroneRecord>(){Constants.TestRecord};
        mockFleet.Setup(x => x.GetAllAsync()).Returns(Task.FromResult(testDb));
        var access = new DatabaseAccess.Controllers.DatabaseAccess(mockLogger.Object, mockFleet.Object, mockOrders.Object);
        var result = await access.GetFleet();
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(testDb);
    }
    
    [Fact]
    public async Task create_order_should_place_order_given_return_empty_obj_for_now()
    {
        var mockLogger = new Mock<ILogger<DatabaseAccess.Controllers.DatabaseAccess>>();
        var mockFleet = new Mock<IFleetRepository>();
        var mockOrders = new Mock<IOrdersRepository>();
        mockOrders.Setup(x => x.CreateAsync(Constants.TestOrder)).Verifiable();
        var access = new DatabaseAccess.Controllers.DatabaseAccess(mockLogger.Object, mockFleet.Object, mockOrders.Object);
        var result = await access.CreateOrder(Constants.TestOrder);
        mockOrders.Verify();
        result.Should().NotBeNull();
        result.IsCompletedSuccessfullly.Should().BeFalse();
    }
    
    [Fact]
    public void get_drone_should_pull_a_record()
    {
        var mockLogger = new Mock<ILogger<DatabaseAccess.Controllers.DatabaseAccess>>();
        var mockFleet = new Mock<IFleetRepository>();
        var mockOrders = new Mock<IOrdersRepository>();
        var testDb = new List<DroneRecord>() {Constants.TestRecord};
        mockFleet.Setup(x => x.GetByIdAsync(It.IsAny<string>())).Returns((string x) => Task.FromResult(testDb.Find(y => y.DroneId == x)));
        var access = new DatabaseAccess.Controllers.DatabaseAccess(mockLogger.Object, mockFleet.Object, mockOrders.Object);
        var result = access.GetDrone(Constants.DroneId);
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(Constants.TestRecord);
    }
    
    [Fact]
    public void get_order_should_pull_an_order()
    {
        var mockLogger = new Mock<ILogger<DatabaseAccess.Controllers.DatabaseAccess>>();
        var mockFleet = new Mock<IFleetRepository>();
        var mockOrders = new Mock<IOrdersRepository>();
        var testDb = new List<Order>() {Constants.TestOrder};
        mockOrders.Setup(x => x.GetByIdAsync(It.IsAny<string>())).Returns((string x) => Task.FromResult(testDb.Find(y => y.Id == x)));
        var access = new DatabaseAccess.Controllers.DatabaseAccess(mockLogger.Object, mockFleet.Object, mockOrders.Object);
        var result = access.GetOrder(Constants.TestOrderId);
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(Constants.TestOrder);
    }
}