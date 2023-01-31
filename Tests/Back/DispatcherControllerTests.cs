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

namespace Tests.Back;

public class DispatcherControllerTests
{
    
    private List<DeliveryEntity> _testDeliveryDb = new (){Constants.TestDeliveryEntity};
    private List<DroneEntity> _testFleetDb = new () {Constants.Entity};
    private void FinishDelivery()
    {
        _testDeliveryDb.ForEach(x => x.TimeDelivered = DateTime.Now);
    }

    private void TestAddDrone()
    {
        _testFleetDb.Add(Constants.Entity);
    }
    private void TestFlipDrone()
    {
        _testFleetDb.ForEach(x => x.LatestStatus = DroneStatus.Ready);
    }

    private void SetupEnvironment()
    {
        _testDeliveryDb = new List<DeliveryEntity>() {};
        _testFleetDb = new List<DroneEntity>() {};
        Environment.SetEnvironmentVariable("DISPATCH_URL", "bogus");
        Environment.SetEnvironmentVariable("HOME_LATITUDE", "0");
        Environment.SetEnvironmentVariable("HOME_LONGITUDE", "0");
    }
    // [Fact]
    // public void add_drone_should_create_drone_and_respond()
    // {
    //     SetupEnvironment();
    //     var mockFleetRepo = new Mock<IFleetRepository>();
    //     var mockDeliveryRepo = new Mock<IDeliveriesRepository>();
    //     var mockGateway = new Mock<IDispatchToSimDroneGateway>();
    //     var mockResult = new Mock<UpdateResult>();
    //     mockGateway.Setup(x => x.InitDrone(It.IsAny<InitDroneRequest>())).Returns(Task.FromResult(Constants.TestInitDroneResponse));
    //     mockGateway.Setup(x => x.AssignFleet(It.IsAny<AssignFleetRequest>()))
    //         .Returns(Task.FromResult(Constants.TestAssignFleetResponse));
    //     mockFleetRepo.Setup(x => x.GetAllAsync()).Returns(Task.FromResult(_testFleetDb));
    //     mockFleetRepo.Setup(x => x.CreateAsync(It.IsAny<DroneModel>())).Returns(Task.CompletedTask).Callback(TestAddDrone);
    //     mockFleetRepo.Setup(x => x.UpdateAsync(It.IsAny<DroneUpdate>())).Returns(Task.FromResult(mockResult.Object)).Callback(TestAddDrone);
    //     var mockODDS = new Mock<IODDSSettings>();
    //     mockODDS.Setup(x => x.GetFleetCollection()).Returns(mockFleetRepo.Object);
    //     mockODDS.Setup(x => x.GetDeliveriesCollection()).Returns(mockDeliveryRepo.Object); 
    //     var dispatcher = new DispatchController(mockODDS.Object);
    //     dispatcher.ChangeGateway(mockGateway.Object);
    //     // Have to make new model since it mutates reference given
    //     var resultTask = dispatcher.AddDrone(new DroneModel
    //     {
    //         CurrentLocation = Constants.TestModel.CurrentLocation,
    //         Destination = Constants.TestModel.Destination,
    //         DispatchUrl = Constants.TestModel.DispatchUrl,
    //         DroneId = Constants.TestModel.DroneId,
    //         DroneUrl = Constants.TestModel.DroneUrl,
    //         HomeLocation = Constants.TestModel.HomeLocation,
    //         Id = Constants.TestModel.Id,
    //         DeliveryId = Constants.TestModel.DeliveryId,
    //         Status = Constants.TestModel.Status
    //     });
    //     resultTask.Wait();
    //     _testFleetDb.Count.Should().BeGreaterThan(0);
    //     // One of the only non-mutated members
    //     _testFleetDb[0].DroneUrl.Should().BeEquivalentTo(Constants.TestAddDroneRequest.DroneUrl);
    // } // Dereferences Mock of drone at some point when ran async

    [Fact]
    public void complete_valid_delivery_should_be_true()
    {
        SetupEnvironment();
        var mockResult = new Mock<UpdateResult>(); // nasty Mongo object mocked for use to ignore update
        mockResult.Setup(x => x.IsAcknowledged).Returns(true);
        _testDeliveryDb.Add(Constants.TestDeliveryEntity);
        var mockFleetRepo = new Mock<IDroneRepository>();
        var mockDeliveryRepo = new Mock<IDeliveriesRepository>();
        mockDeliveryRepo.Setup(x => x.UpdateAsync(It.IsAny<DeliveryUpdate>()))
            .Returns(Task.FromResult(mockResult.Object)).Callback(FinishDelivery);
        var mockODDS = new Mock<IODDSSettings>();
        mockODDS.Setup(x => x.GetFleetCollection()).Returns(mockFleetRepo.Object);
        mockODDS.Setup(x => x.GetDeliveriesCollection()).Returns(mockDeliveryRepo.Object);
        var dispatcher = new DispatchController(mockODDS.Object);
        var responseTask = dispatcher.CompleteDelivery(Constants.TestCompleteDeliveryRequest);
        responseTask.Wait();
        var response = responseTask.Result;
        response.Should().BeEquivalentTo(Constants.TestCompleteDeliveryResponse);
    }

    [Fact]
    public void get_drone_by_id_should_get_drone_model()
    {
        SetupEnvironment();
        _testFleetDb.Add(Constants.Entity);
        var mockGateway = new Mock<IDispatchToSimDroneGateway>();
        var mockFleetRepo = new Mock<IDroneRepository>();
        var mockDeliveryRepo = new Mock<IDeliveriesRepository>();
        mockFleetRepo.Setup(x => x.GetByIdAsync(It.IsAny<string>())).Returns<string>(y => Task.FromResult(_testFleetDb.FindLast(x => x.DroneId == y)));
        var mockODDS = new Mock<IODDSSettings>();
        mockODDS.Setup(x => x.GetFleetCollection()).Returns(mockFleetRepo.Object);
        mockODDS.Setup(x => x.GetDeliveriesCollection()).Returns(mockDeliveryRepo.Object);
        var dispatcher = new DispatchController(mockODDS.Object);
        dispatcher.ChangeGateway(mockGateway.Object);
        var responseTask = dispatcher.GetDroneById(Constants.Entity.DroneId);
        responseTask.Wait();
        var response = responseTask.Result;
        response.Value.Should().NotBeNull();
        response.Value.Should().BeEquivalentTo(Constants.Entity);
    }
    
    [Fact]
    public void revive_should_change_db_model_to_ready()
    {
        SetupEnvironment();
        var mockMongoQuery = new Mock<UpdateResult>();
        // model made this way since updateDrone mutates by reference
        var deadModel = new DroneEntity
        {
            CurrentLocation = Constants.TestEntityDead.CurrentLocation,
            Destination = Constants.TestEntityDead.Destination,
            DispatchUrl = Constants.TestEntityDead.DispatchUrl,
            DroneId = Constants.TestEntityDead.DroneId,
            DeliveryId = Constants.TestEntityDead.DeliveryId,
            LatestStatus = Constants.TestEntityDead.LatestStatus,
            DroneUrl = Constants.TestEntityDead.DroneUrl,
            HomeLocation = Constants.TestEntityDead.HomeLocation,
            Id = Constants.TestEntityDead.Id
        };
        _testFleetDb.Add(deadModel);
        var mockFleetRepo = new Mock<IDroneRepository>();
        var mockDeliveryRepo = new Mock<IDeliveriesRepository>();
        mockFleetRepo.Setup(x => x.GetAllAsync()).Returns(Task.FromResult(_testFleetDb));
        mockFleetRepo.Setup(x => x.UpdateAsync(It.IsAny<DroneUpdate>())).Returns(Task.FromResult(mockMongoQuery.Object)).Callback(TestFlipDrone);
        var mockODDS = new Mock<IODDSSettings>();
        mockODDS.Setup(x => x.GetFleetCollection()).Returns(mockFleetRepo.Object);
        mockODDS.Setup(x => x.GetDeliveriesCollection()).Returns(mockDeliveryRepo.Object);
        var dispatcher = new DispatchController(mockODDS.Object);
        var responseTask = dispatcher.Revive(deadModel);
        responseTask.Wait();
        var response = responseTask.Result;
        response.Should().BeFalse();
        // Unsuccessful instead of successful being the return is interesting
        _testFleetDb.Should().Contain(Constants.Entity);
        _testFleetDb.Should().NotContain(Constants.TestEntityDead);
    }
    
    [Fact]
    public void assign_check_should_pull_delivery_and_drone_and_initiate_delivery()
    {
        SetupEnvironment();
        var mockMongoQuery = new Mock<UpdateResult>();
        _testDeliveryDb.Add(Constants.TestDeliveryEntity);
        _testFleetDb.Add(Constants.Entity);
        var mockGateway = new Mock<IDispatchToSimDroneGateway>();
        var mockFleetRepo = new Mock<IDroneRepository>();
        var mockDeliveryRepo = new Mock<IDeliveriesRepository>();
        mockGateway.Setup(x => x.AssignDelivery(It.IsAny<AssignDeliveryRequest>())).Returns(Task.FromResult(Constants.TestAssignDeliveryResponse));
        mockGateway.Setup(x => x.HealthCheck(It.IsAny<string>())).Returns(Task.FromResult(true));
        mockFleetRepo.Setup(x => x.GetByIdAsync(It.IsAny<string>())).Returns<string>(y => Task.FromResult(_testFleetDb.FindLast(x => x.DroneId == y)));
        mockDeliveryRepo.Setup(x => x.GetByIdAsync(It.IsAny<string>())).Returns<string>(y => Task.FromResult(_testDeliveryDb.FindLast(x => x.DroneId == y)));
        mockFleetRepo.Setup(x => x.GetAllAsync()).Returns(Task.FromResult(_testFleetDb));
        mockDeliveryRepo.Setup(x => x.GetAllAsync()).Returns(Task.FromResult(_testDeliveryDb));
        mockDeliveryRepo.Setup(x => x.UpdateAsync(It.IsAny<DeliveryUpdate>())).Returns(Task.FromResult(mockMongoQuery.Object));
        mockFleetRepo.Setup(x => x.UpdateAsync(It.IsAny<DroneUpdate>())).Returns(Task.FromResult(mockMongoQuery.Object));
        var mockODDS = new Mock<IODDSSettings>();
        mockODDS.Setup(x => x.GetFleetCollection()).Returns(mockFleetRepo.Object);
        mockODDS.Setup(x => x.GetDeliveriesCollection()).Returns(mockDeliveryRepo.Object);
        var dispatcher = new DispatchController(mockODDS.Object);
        dispatcher.ChangeGateway(mockGateway.Object);
        var responseTask = dispatcher.AssignmentCheck(Constants.TestPingDto);
        responseTask.Wait();
        var response = responseTask.Result;
        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(Constants.TestPingDto);
    }

    [Fact]
    public void post_init_status_should_update_starting_fleet_db()
    {
        SetupEnvironment();
        var mockMongoQuery = new Mock<UpdateResult>();
        // model made this way since updateDrone mutates by reference
        var deadModel = new DroneEntity
        {
            CurrentLocation = Constants.TestEntityDead.CurrentLocation,
            Destination = Constants.TestEntityDead.Destination,
            DispatchUrl = Constants.TestEntityDead.DispatchUrl,
            DroneId = Constants.TestEntityDead.DroneId,
            DeliveryId = Constants.TestEntityDead.DeliveryId,
            LatestStatus = Constants.TestEntityDead.LatestStatus,
            DroneUrl = Constants.TestEntityDead.DroneUrl,
            HomeLocation = Constants.TestEntityDead.HomeLocation,
            Id = Constants.TestEntityDead.Id
        };
        _testFleetDb.Add(deadModel);
        var mockGateway = new Mock<IDispatchToSimDroneGateway>();
        var mockFleetRepo = new Mock<IDroneRepository>();
        var mockDeliveryRepo = new Mock<IDeliveriesRepository>();
        mockMongoQuery.Setup(x => x.IsAcknowledged).Returns(true);
        mockMongoQuery.Setup(x => x.ModifiedCount).Returns(_testFleetDb.Count);
        mockFleetRepo.Setup(x => x.UpdateAsync(It.IsAny<DroneUpdate>())).Returns(Task.FromResult(mockMongoQuery.Object)).Callback(TestFlipDrone);
        var mockODDS = new Mock<IODDSSettings>();
        mockODDS.Setup(x => x.GetFleetCollection()).Returns(mockFleetRepo.Object);
        mockODDS.Setup(x => x.GetDeliveriesCollection()).Returns(mockDeliveryRepo.Object);
        var dispatcher = new DispatchController(mockODDS.Object);
        dispatcher.ChangeGateway(mockGateway.Object);
        var responseTask = dispatcher.PostInitialStatus(Constants.TestUpdate);
        responseTask.Wait();
        var response = responseTask.Result;
        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(Constants.TestUpdateResponse);
        _testFleetDb.Should().Contain(Constants.Entity);
        _testFleetDb.Should().NotContain(Constants.TestEntityDead);
    }

    [Fact]
    public void update_drone_status_should_change_fleet_db()
    {
        SetupEnvironment();
        var mockMongoQuery = new Mock<UpdateResult>();
        // model made this way since updateDrone mutates by reference
        var deadModel = new DroneEntity
        {
            CurrentLocation = Constants.TestEntityDead.CurrentLocation,
            Destination = Constants.TestEntityDead.Destination,
            DispatchUrl = Constants.TestEntityDead.DispatchUrl,
            DroneId = Constants.TestEntityDead.DroneId,
            DeliveryId = Constants.TestEntityDead.DeliveryId,
            LatestStatus = Constants.TestEntityDead.LatestStatus,
            DroneUrl = Constants.TestEntityDead.DroneUrl,
            HomeLocation = Constants.TestEntityDead.HomeLocation,
            Id = Constants.TestEntityDead.Id
        };
        _testFleetDb.Add(deadModel); 
        var mockGateway = new Mock<IDispatchToSimDroneGateway>();
        var mockFleetRepo = new Mock<IDroneRepository>();
        var mockDeliveryRepo = new Mock<IDeliveriesRepository>();
        mockMongoQuery.Setup(x => x.IsAcknowledged).Returns(true);
        mockMongoQuery.Setup(x => x.ModifiedCount).Returns(_testFleetDb.Count);
        mockFleetRepo.Setup(x => x.UpdateAsync(It.IsAny<DroneUpdate>())).Returns(Task.FromResult(mockMongoQuery.Object)).Callback(TestFlipDrone);
        var mockODDS = new Mock<IODDSSettings>();
        mockODDS.Setup(x => x.GetFleetCollection()).Returns(mockFleetRepo.Object);
        mockODDS.Setup(x => x.GetDeliveriesCollection()).Returns(mockDeliveryRepo.Object);
        var dispatcher = new DispatchController(mockODDS.Object);
        dispatcher.ChangeGateway(mockGateway.Object);
        var responseTask = dispatcher.UpdateDroneStatus(Constants.TestUpdate);
        responseTask.Wait();
        var response = responseTask.Result;
        response.Should().NotBeNull();
        response.Should().BeEquivalentTo(Constants.TestUpdateResponse);
    }
    
}
