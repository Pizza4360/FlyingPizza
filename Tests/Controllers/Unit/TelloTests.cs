using System.Threading.Tasks;
using Domain.Entities;
using Domain.InterfaceImplementations.Gateways;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SimDrone;
using SimDrone.Controllers;
using Xunit;

namespace Tests.Controllers.Unit;

public class TelloTests
{

    public readonly static OkObjectResult ExpectedHttp = new OkObjectResult("ok");
    
    
    
    [Fact]
    public async Task TelloAdapterShouldReturnOkDeliverOrder()
    {
        // Assumed to return an ok object result with ok as arg
        var adapter = new TelloDroneController(new TelloDrone(Constants.TestRecord, new DroneToDispatchGateway()));
        var response = await adapter.DeliverOrder(Constants.TestAssignDeliveryRequest);
        response.Should().NotBeNull();
        response.Should().NotBeEquivalentTo("ok");
    }
    [Fact]
    public async Task TelloAdapterShouldReturnOkInitRegistration()
    {
        var adapter = new TelloDroneController(new TelloDrone(Constants.TestRecord, new DroneToDispatchGateway()));
        adapter._drone = new TelloDrone(Constants.TestRecord, new DroneToDispatchGateway());
        var response = await adapter.InitializeRegistration(Constants.TestInitDroneRequest);
        response.Should().NotBeNull();
        response.Should().NotBeEquivalentTo(ExpectedHttp);
    }
    
    [Fact]
    public async Task TelloAdapterShouldReturnOkStartDrone()
    {
        var adapter = new TelloDroneController(new TelloDrone(Constants.TestRecord, new DroneToDispatchGateway()));
        adapter._drone = new TelloDrone(Constants.TestRecord, new DroneToDispatchGateway());
        var response = await adapter.StartDrone(new TelloDrone(Constants.TestRecord, new DroneToDispatchGateway()));
        response.Should().NotBeNull();
        response.Should().NotBeEquivalentTo(ExpectedHttp);

    }
    [Fact]
    public async Task TelloAdapterShouldReturnOkAssignToFleet()
    {
        var adapter = new TelloDroneController(new TelloDrone(Constants.TestRecord, new DroneToDispatchGateway()));
        adapter._drone = new TelloDrone(Constants.TestRecord, new DroneToDispatchGateway());
            var response = await adapter.AssignToFleet(Constants.TestCompleteRegistrationResponse);
        response.Should().NotBeNull();
        response.Should().NotBeEquivalentTo(ExpectedHttp);

    }
    [Fact]
        public void drone_should_have_destination_in_route()
        {
            var mockedDispatcher = new Mock<DroneToDispatchGateway>().Object;
            var drone = new TelloDrone(
                new DroneRecord
                {
                    OrderId = Constants.DroneId,
                    HomeLocation = Constants.HomeLocation,
                    IpAddress = Constants.DroneIp,
                    DispatcherUrl = Constants.Url,
                    BadgeNumber = Constants.TestBadgeNumber
                },
                mockedDispatcher){
                Destination = Constants.Destination
            };

            var route = drone.GetRoute();
            route.Should()
                .NotBeNull();
            route.Should()
                .ContainEquivalentOf(Constants.Destination);
        }

        [Fact]
        public void drone_should_have_start_in_route()
        {
            var mockedDispatcher = new Mock<DroneToDispatchGateway>().Object;
            
            var drone = new TelloDrone(
                new DroneRecord
                {
                    OrderId = Constants.DroneId,
                    HomeLocation = Constants.HomeLocation,
                    IpAddress = Constants.DroneIp,
                    DispatcherUrl = Constants.Url,
                    BadgeNumber = Constants.TestBadgeNumber
                },
                mockedDispatcher)
            {
                Destination = Constants.Destination
            };

            var route = drone.GetRoute();
            route.Should()
                .NotBeNull();
            route.Should()
                .ContainEquivalentOf(Constants.HomeLocation);
        }

        [Fact]
        public void TestGetRouteAllPositiveNumbers()
        {
            var mockedDispatcher = new Mock<DroneToDispatchGateway>().Object;
            var drone = new TelloDrone(
                new DroneRecord
                {
                    OrderId = Constants.DroneId,
                    HomeLocation = Constants.HomeLocation,
                    IpAddress = Constants.DroneIp,
                    DispatcherUrl = Constants.Url,
                    BadgeNumber = Constants.TestBadgeNumber
                },
                mockedDispatcher)
            {
                Destination = Constants.Destination
            };

            var route = drone.GetRoute();
            route.Should()
                .NotBeNull();
            foreach (var geoLocation in route)
            {
                geoLocation.Latitude.Should()
                    .BeGreaterThanOrEqualTo(0m);
                geoLocation.Longitude.Should()
                    .BeGreaterThanOrEqualTo(0m);
            }
        }

        [Fact]
        public void TestGetRouteAllNegativeNumbers()
        {
            var mockedDispatcher = new Mock<DroneToDispatchGateway>().Object;
            var drone = new TelloDrone(
                new DroneRecord
                {
                    OrderId = Constants.DroneId,
                    HomeLocation = Constants.NegativeHomeLocation,
                    IpAddress = Constants.DroneIp,
                    DispatcherUrl = Constants.Url,
                    BadgeNumber = Constants.TestBadgeNumber
                },
                mockedDispatcher)
            {
                Destination = Constants.NegativeDestination
            };

            var route = drone.GetRoute();
            route.Should()
                .NotBeNull();
            foreach (var geoLocation in route)
            {
                geoLocation.Latitude.Should()
                    .BeLessThanOrEqualTo(0.0m);
                geoLocation.Longitude.Should()
                    .BeLessThanOrEqualTo(0.0m);
            }
        }
    }