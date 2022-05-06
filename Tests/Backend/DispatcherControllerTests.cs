using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Dispatch.Controllers;
using Domain.DTO;
using Domain.DTO.DroneDispatchCommunication;
using Domain.Entities;
using Domain.GatewayDefinitions;
using Domain.RepositoryDefinitions;
using FluentAssertions;
using MongoDB.Driver;
using Moq;
using Xunit;

namespace Tests.Backend;

public class DispatcherControllerTests
{
    
    private List<Order> _testOrderDb = new (){Constants.TestOrder};
    private List<DroneRecord> _testFleetDb = new () {Constants.TestRecord};
    private void FinishOrder()
    {
        _testOrderDb.ForEach(x => x.TimeDelivered = DateTime.Now);
    }

    private void TestAddDrone()
    {
        _testFleetDb.Add(Constants.TestRecord);
    }
    private void TestFlipDrone()
    {
        _testFleetDb.ForEach(x => x.State = DroneState.Ready);
    }

    private void SetupEnvironment()
    {
        Environment.SetEnvironmentVariable("DISPATCH_URL", "bogus");
        Environment.SetEnvironmentVariable("HOME_LATITUDE", "0");
        Environment.SetEnvironmentVariable("HOME_LONGITUDE", "0");
    }
    [Fact]
    public async Task add_drone_should_create_drone_and_respond()
    {
        SetupEnvironment();
        var mockFleetRepo = new Mock<IFleetRepository>();
        var mockOrderRepo = new Mock<IOrdersRepository>();
        var mockGateway = new Mock<IDispatchToSimDroneGateway>();
        var dispatcher = new DispatchController(mockFleetRepo.Object,mockOrderRepo.Object);
        _testFleetDb = new List<DroneRecord>(){};
        mockGateway.Setup(x => x.InitDrone(It.IsAny<InitDroneRequest>())).Returns(Task.FromResult(Constants.TestInitDroneResponse));
        mockGateway.Setup(x => x.AssignFleet(It.IsAny<AssignFleetRequest>()))
            .Returns(Task.FromResult(Constants.TestAssignFleetResponse));
        mockFleetRepo.Setup(x => x.GetAllAsync()).Returns(Task.FromResult(_testFleetDb));
        mockFleetRepo.Setup(x => x.CreateAsync(It.IsAny<DroneRecord>())).Returns(Task.CompletedTask).Callback(TestAddDrone);
        dispatcher.ChangeGateway(mockGateway.Object);
        var response = await dispatcher.AddDrone(Constants.TestAddDroneRequest);
        response.Should().NotBeNull();
        response.Success.Should().BeTrue();
        _testFleetDb.Count.Should().BeGreaterThan(0);
        // One of the only non-mutated members
        _testFleetDb[0].DroneUrl.Should().BeEquivalentTo(Constants.TestAddDroneRequest.DroneUrl);
    }

    [Fact]
    public async Task complete_valid_order_should_be_true()
    {
        SetupEnvironment();
        var mockResult = new Mock<UpdateResult>(); // nasty Mongo object mocked for use to ignore update
        mockResult.Setup(x => x.IsAcknowledged).Returns(true);
        _testOrderDb = new List<Order>(){Constants.TestOrder};
        var mockFleetRepo = new Mock<IFleetRepository>();
        var mockOrderRepo = new Mock<IOrdersRepository>();
        mockOrderRepo.Setup(x => x.UpdateAsync(It.IsAny<OrderUpdate>()))
            .Returns(Task.FromResult(mockResult.Object)).Callback(FinishOrder);
        var dispatcher = new DispatchController(mockFleetRepo.Object,mockOrderRepo.Object);
        var response = await dispatcher.CompleteOrder(Constants.TestCompleteOrderRequest);
        response.Should().BeEquivalentTo(Constants.TestCompleteOrderResponse);
    }

    [Fact]
    public async Task get_drone_by_id_should_get_drone_record()
    {
        SetupEnvironment();
        _testFleetDb = new List<DroneRecord>(){Constants.TestRecord};
        var mockGateway = new Mock<IDispatchToSimDroneGateway>();
        var mockFleetRepo = new Mock<IFleetRepository>();
        var mockOrderRepo = new Mock<IOrdersRepository>();
        mockFleetRepo.Setup(x => x.GetByIdAsync(It.IsAny<string>())).Returns<string>(y => Task.FromResult(_testFleetDb.FindLast(x => x.DroneId == y)));
        var dispatcher = new DispatchController(mockFleetRepo.Object, mockOrderRepo.Object);
       dispatcher.ChangeGateway(mockGateway.Object);
        var response = await dispatcher.GetDroneById(Constants.TestRecord.DroneId);
        response.Value.Should().NotBeNull();
        response.Value.Should().BeEquivalentTo(Constants.TestRecord);
    }
    
    [Fact]
    public async Task revive_should_change_db_record_to_ready()
    {
        SetupEnvironment();
        var mockMongoQuery = new Mock<UpdateResult>();
        // record made this way since updateDrone mutates by reference
        var deadRecord = new DroneRecord
        {
            CurrentLocation = Constants.TestRecordDead.CurrentLocation,
            Destination = Constants.TestRecordDead.Destination,
            DispatchUrl = Constants.TestRecordDead.DispatchUrl,
            DroneId = Constants.TestRecordDead.DroneId,
            OrderId = Constants.TestRecordDead.OrderId,
            State = Constants.TestRecordDead.State,
            DroneUrl = Constants.TestRecordDead.DroneUrl,
            HomeLocation = Constants.TestRecordDead.HomeLocation,
            Id = Constants.TestRecordDead.Id
        };
        _testFleetDb = new List<DroneRecord>(){deadRecord};
        var mockFleetRepo = new Mock<IFleetRepository>();
        var mockOrderRepo = new Mock<IOrdersRepository>();
        mockFleetRepo.Setup(x => x.GetAllAsync()).Returns(Task.FromResult(_testFleetDb));
        mockFleetRepo.Setup(x => x.UpdateAsync(It.IsAny<DroneUpdate>())).Returns(Task.FromResult(mockMongoQuery.Object)).Callback(TestFlipDrone);
        var dispatcher = new DispatchController(mockFleetRepo.Object, mockOrderRepo.Object);
        var response = await dispatcher.Revive(deadRecord);
        response.Should().BeFalse();
        // Unsuccessful instead of successful being the return is interesting
        _testFleetDb.Should().Contain(Constants.TestRecord);
        _testFleetDb.Should().NotContain(Constants.TestRecordDead);
    }
    
    [Fact]
    public async Task assign_check_should_pull_order_and_drone_and_initiate_delivery()
    {
        SetupEnvironment();
        var mockMongoQuery = new Mock<UpdateResult>();
        _testOrderDb = new List<Order>() {Constants.TestOrder};
        _testFleetDb = new List<DroneRecord>(){Constants.TestRecord};
        var mockGateway = new Mock<IDispatchToSimDroneGateway>();
        var mockFleetRepo = new Mock<IFleetRepository>();
        var mockOrderRepo = new Mock<IOrdersRepository>();
        mockGateway.Setup(x => x.AssignDelivery(It.IsAny<AssignDeliveryRequest>())).Returns(Task.FromResult(Constants.TestAssignDeliveryResponse));
        mockGateway.Setup(x => x.HealthCheck(It.IsAny<string>())).Returns(Task.FromResult(true));
        mockFleetRepo.Setup(x => x.GetByIdAsync(It.IsAny<string>())).Returns<string>(y => Task.FromResult(_testFleetDb.FindLast(x => x.DroneId == y)));
        mockOrderRepo.Setup(x => x.GetByIdAsync(It.IsAny<string>())).Returns<string>(y => Task.FromResult(_testOrderDb.FindLast(x => x.DroneId == y)));
        mockFleetRepo.Setup(x => x.GetAllAsync()).Returns(Task.FromResult(_testFleetDb));
        mockOrderRepo.Setup(x => x.GetAllAsync()).Returns(Task.FromResult(_testOrderDb));
        mockOrderRepo.Setup(x => x.UpdateAsync(It.IsAny<OrderUpdate>())).Returns(Task.FromResult(mockMongoQuery.Object));
        mockFleetRepo.Setup(x => x.UpdateAsync(It.IsAny<DroneUpdate>())).Returns(Task.FromResult(mockMongoQuery.Object));
        var dispatcher = new DispatchController(mockFleetRepo.Object, mockOrderRepo.Object);
        dispatcher.ChangeGateway(mockGateway.Object);
        var response = await dispatcher.AssignmentCheck(Constants.TestPingDto);
        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(Constants.TestPingDto);
    }

    [Fact]
    public async Task post_init_status_should_update_starting_fleet_db()
    {
        SetupEnvironment();
        var mockMongoQuery = new Mock<UpdateResult>();
        // record made this way since updateDrone mutates by reference
        var deadRecord = new DroneRecord
        {
            CurrentLocation = Constants.TestRecordDead.CurrentLocation,
            Destination = Constants.TestRecordDead.Destination,
            DispatchUrl = Constants.TestRecordDead.DispatchUrl,
            DroneId = Constants.TestRecordDead.DroneId,
            OrderId = Constants.TestRecordDead.OrderId,
            State = Constants.TestRecordDead.State,
            DroneUrl = Constants.TestRecordDead.DroneUrl,
            HomeLocation = Constants.TestRecordDead.HomeLocation,
            Id = Constants.TestRecordDead.Id
        };
        _testFleetDb = new List<DroneRecord>(){deadRecord}; var mockGateway = new Mock<IDispatchToSimDroneGateway>();
        var mockFleetRepo = new Mock<IFleetRepository>();
        var mockOrderRepo = new Mock<IOrdersRepository>();
        mockMongoQuery.Setup(x => x.IsAcknowledged).Returns(true);
        mockMongoQuery.Setup(x => x.ModifiedCount).Returns(_testFleetDb.Count);
        mockFleetRepo.Setup(x => x.UpdateAsync(It.IsAny<DroneUpdate>())).Returns(Task.FromResult(mockMongoQuery.Object)).Callback(TestFlipDrone);
        var dispatcher = new DispatchController(mockFleetRepo.Object, mockOrderRepo.Object);
        dispatcher.ChangeGateway(mockGateway.Object);
        var response = await dispatcher.PostInitialStatus(Constants.TestUpdate);
        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(Constants.TestUpdateResponse);
        _testFleetDb.Should().Contain(Constants.TestRecord);
        _testFleetDb.Should().NotContain(Constants.TestRecordDead);
    }

    [Fact]
    public async Task update_drone_status_should_change_fleet_db()
    {
        SetupEnvironment();
        var mockMongoQuery = new Mock<UpdateResult>();
        // record made this way since updateDrone mutates by reference
        var deadRecord = new DroneRecord
        {
            CurrentLocation = Constants.TestRecordDead.CurrentLocation,
            Destination = Constants.TestRecordDead.Destination,
            DispatchUrl = Constants.TestRecordDead.DispatchUrl,
            DroneId = Constants.TestRecordDead.DroneId,
            OrderId = Constants.TestRecordDead.OrderId,
            State = Constants.TestRecordDead.State,
            DroneUrl = Constants.TestRecordDead.DroneUrl,
            HomeLocation = Constants.TestRecordDead.HomeLocation,
            Id = Constants.TestRecordDead.Id
        };
        _testFleetDb = new List<DroneRecord>(){deadRecord}; 
        var mockGateway = new Mock<IDispatchToSimDroneGateway>();
        var mockFleetRepo = new Mock<IFleetRepository>();
        var mockOrderRepo = new Mock<IOrdersRepository>();
        mockMongoQuery.Setup(x => x.IsAcknowledged).Returns(true);
        mockMongoQuery.Setup(x => x.ModifiedCount).Returns(_testFleetDb.Count);
        mockFleetRepo.Setup(x => x.UpdateAsync(It.IsAny<DroneUpdate>())).Returns(Task.FromResult(mockMongoQuery.Object)).Callback(TestFlipDrone);
        var dispatcher = new DispatchController(mockFleetRepo.Object, mockOrderRepo.Object);
        dispatcher.ChangeGateway(mockGateway.Object);
        var response = await dispatcher.UpdateDroneStatus(Constants.TestUpdate);
        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(Constants.TestUpdateResponse);
    }
    
}