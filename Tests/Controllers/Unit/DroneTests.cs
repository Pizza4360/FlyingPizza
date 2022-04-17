using Domain.DTO;
using Domain.Entities;
using Domain.InterfaceImplementations.Gateways;
using FluentAssertions;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace Tests.Controllers.Unit
{
    public class DroneTests
    {
        // Helper method for console output during testing.
        private readonly ITestOutputHelper _testOutputHelper;

        public DroneTests(
        ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void drone_should_have_destination_in_route()
        {
            var mockedDispatcher = new Mock<DroneToDispatchGateway>().Object;
            var drone = new SimDrone.Drone(
                new DroneRecord
                {
                    OrderId = Constants.DroneId,
                    HomeLocation = Constants.HomeLocation,
                    IpAddress = Constants.DroneIp,
                    DispatcherUrl = Constants.Url,
                    BadgeNumber = 1
                },
                mockedDispatcher);

            var route = drone.GetRoute();
            route.Should()
                .NotBeNull();
            route.Should()
                .Contain(Constants.Destination);
        }

        [Fact]
        public void drone_should_have_start_in_route()
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
            var drone = new SimDrone.Drone(
                new DroneRecord
                {
                    OrderId = Constants.DroneId,
                    HomeLocation = Constants.HomeLocation,
                    IpAddress = Constants.DroneIp,
                    DispatcherUrl = Constants.Url,
                    BadgeNumber = 1
                },
                mockedDispatcher);
            drone.Destination = dest;

            var route = drone.GetRoute();
            route.Should()
                .NotBeNull();
            route.Should()
                .Contain(home);
            //TODO: BUG #4 I am unsure if haversine is supposed to have this behavior but a trip between 0.0 and 3.0 is passing -2574.833, -55345.217
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
            var drone = new SimDrone.Drone(
                new DroneRecord
                {
                    OrderId = Constants.DroneId,
                    HomeLocation = Constants.HomeLocation,
                    IpAddress = Constants.DroneIp,
                    DispatcherUrl = Constants.Url,
                    BadgeNumber = 1
                },
                mockedDispatcher);
            drone.Destination = dest;

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
            var home = new GeoLocation
            {
                Latitude = 0.0m,
                Longitude = 0.0m
            };
            var dest = new GeoLocation
            {
                Latitude = -3.0m,
                Longitude = -4.0m
            };
            var drone = new SimDrone.Drone(
                new DroneRecord
                {
                    OrderId = Constants.DroneId,
                    HomeLocation = Constants.HomeLocation,
                    IpAddress = Constants.DroneIp,
                    DispatcherUrl = Constants.Url,
                    BadgeNumber = 1
                },
                mockedDispatcher);
            drone.Destination = dest;

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
