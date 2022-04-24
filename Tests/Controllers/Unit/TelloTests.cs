using Domain.DTO;
using Domain.Entities;
using Domain.InterfaceImplementations.Gateways;
using FluentAssertions;
using Moq;
using SimDrone;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Controllers.Unit
{
    public class TelloTests
    {
        // Helper method for console output during testing.
        private readonly ITestOutputHelper _testOutputHelper;

        public TelloTests(
        ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void tello_drone_should_have_destination_in_route()
        {
            var mockedDispatcher = new Mock<DroneToDispatchGateway>().Object;
            var drone = new TelloDrone(
                Constants.TestRecord,
                mockedDispatcher);

            var route = drone.GetRoute();
            route.Should()
                .NotBeNull();
            route.Should()
                .Contain(Constants.Destination);
        }

        [Fact]
        public void tello_drone_should_have_start_in_route()
        {
            var mockedDispatcher = new Mock<DroneToDispatchGateway>().Object;
            var drone = new TelloDrone(
                Constants.TestRecord,
                mockedDispatcher);
            drone.Destination = Constants.Destination;

            var route = drone.GetRoute();
            route.Should()
                .NotBeNull();
            route.Should()
                .Contain(Constants.HomeLocation);
        }

        [Fact]
        public void TestGetRouteAllPositiveNumbers()
        {
            var mockedDispatcher = new Mock<DroneToDispatchGateway>().Object;
            var home = new GeoLocation
            {
                Latitude = 0.0m,
                Longitude = 0.0m
            };
            var dest = new GeoLocation
            {
                Latitude = 3.0m,
                Longitude = 4.0m
            };
            var drone = new TelloDrone(
                Constants.TestRecord,
                mockedDispatcher);
            drone.Destination = dest;
            drone.HomeLocation = home;

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
        public void TestTelloGetRouteAllNegativeNumbers()
        {
            var mockedDispatcher = new Mock<DroneToDispatchGateway>().Object;
            var home = new GeoLocation
            {
                Latitude = -1.0m,
                Longitude = -1.0m
            };
            var dest = new GeoLocation
            {
                Latitude = -3.0m,
                Longitude = -4.0m
            };
            var drone = new TelloDrone(new DroneRecord
                {
                    BadgeNumber = Constants.TestBadgeNumber,
                    CurrentLocation = home, 
                    Destination = dest,
                    DispatcherUrl = Constants.DispatcherIp,
                    DroneIp = Constants.DroneIp,
                    HomeLocation = home,
                    Id = Constants.DroneId,
                    OrderId = Constants.TestOrderId,
                    State = DroneState.Ready
                },
                mockedDispatcher);
            drone.Destination = dest;
            drone.HomeLocation = home;

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
}
