using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.Entities;
using Domain.RepositoryDefinitions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Tests.Back;

public class DatabaseAccessTests
{
    [Fact]
    public async Task get_fleet_should_get_list_of_drone_models()
    {
        var mockDeliveries = new Mock<IDeliveriesRepository>();
        var mockFleet = new Mock<IDroneRepository>();
        var testDb = new List<DroneEntity>(){Constants.Entity};
        mockFleet.Setup(x => x.GetAllAsync()).Returns(Task.FromResult(testDb));
        var mockODDS = new Mock<IODDSSettings>();
        mockODDS.Setup(x => x.GetFleetCollection()).Returns(mockFleet.Object);
        mockODDS.Setup(x => x.GetDeliveriesCollection()).Returns(mockDeliveries.Object);
        var access = new DatabaseAccess.Controllers.DatabaseAccess(mockODDS.Object);
        var result = await access.GetFleet();
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(testDb);
    }
    
    // [Fact]
    // public async Task create_delivery_should_place_delivery_given_return_empty_obj_for_now()
    // {
    //     var handleFactory = new HttpHandlerFactory();
    //     handleFactory.StringAll();
    //     var handle = handleFactory.GetHttpMessageHandler();
    //     var mockLogger = new Mock<ILogger<DatabaseAccess.Controllers.DatabaseAccess>>();
    //     var mockFleet = new Mock<IFleetRepository>();
    //     var mockDeliveries = new Mock<IDeliveriesRepository>();
    //     LocationParser.ChangeHandler(handle);
    //     mockDeliveries.Setup(x => x.CreateAsync(Constants.TestDelivery)).Verifiable();
    //     mockDeliveries.Setup(x => x.CreateAsync(It.IsAny<Delivery>())).Returns(Task.CompletedTask);
    //     var mockODDS = new Mock<IODDSSettings>();
    //     mockODDS.Setup(x => x.GetFleetCollection()).Returns(mockFleet.Object);
    //     mockODDS.Setup(x => x.GetDeliveriesCollection()).Returns(mockDeliveries.Object);
    //     var access = new DatabaseAccess.Controllers.DatabaseAccess(mockODDS.Object);
    //     var result = await access.CreateDelivery(Constants.TestDelivery);
    //     mockDeliveries.Verify();
    //     result.Should().NotBeNull();
    //     result.IsCompletedSuccessfullly.Should().BeFalse();
    //     // Not Testable, HttpSocket can't be mocked here
    // }
    
    [Fact]
    public void get_drone_should_pull_a_model()
    {
        var mockLogger = new Mock<ILogger<DatabaseAccess.Controllers.DatabaseAccess>>();
        var mockFleet = new Mock<IDroneRepository>();
        var mockDeliveries = new Mock<IDeliveriesRepository>();
        var testDb = new List<DroneEntity>() {Constants.Entity};
        mockFleet.Setup(x => x.GetByIdAsync(It.IsAny<string>())).Returns((string x) => Task.FromResult(testDb.Find(y => y.DroneId == x)));
        var mockODDS = new Mock<IODDSSettings>();
        mockODDS.Setup(x => x.GetFleetCollection()).Returns(mockFleet.Object);
        mockODDS.Setup(x => x.GetDeliveriesCollection()).Returns(mockDeliveries.Object);
        var access = new DatabaseAccess.Controllers.DatabaseAccess(mockODDS.Object);
        var result = access.GetDrone(Constants.DroneId);
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(Constants.Entity);
    }
    
    [Fact]
    public void get_delivery_should_pull_an_delivery()
    {
        var mockFleet = new Mock<IDroneRepository>();
        var mockDeliveries = new Mock<IDeliveriesRepository>();
        var testDb = new List<DeliveryEntity>() {Constants.TestDeliveryEntity};
        mockDeliveries.Setup(x => x.GetByIdAsync(It.IsAny<string>())).Returns((string x) => Task.FromResult(testDb.Find(y => y.Id == x)));
        var mockODDS = new Mock<IODDSSettings>();
        mockODDS.Setup(x => x.GetFleetCollection()).Returns(mockFleet.Object);
        mockODDS.Setup(x => x.GetDeliveriesCollection()).Returns(mockDeliveries.Object);
        var access = new DatabaseAccess.Controllers.DatabaseAccess(mockODDS.Object);
        var result = access.GetDeliveryById(Constants.TestDeliveryId);
        result.Should().NotBeNull();
        result.Should().BeEquivalentTo(Constants.TestDeliveryEntity);
    }
}