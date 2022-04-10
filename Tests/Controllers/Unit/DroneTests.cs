/*
using Domain.Entities;
using Domain.Interfaces.Gateways;
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

        public DroneTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }

        [Fact]
        public void drone_should_have_destination_in_route()
        {
            var mockedDispatcher = new Mock<IDispatcherGateway>().Object;
            var drone = new DroneSimulator.Drone(Constants.DroneId, Constants.HomeLocation, mockedDispatcher, 1, Constants.DroneIp, Constants.Url)
                {
                    Destination = Constants.Destination
                };

            var route =  drone.GetRoute();
            route.Should().NotBeNull();
            route.Should().Contain(Constants.Destination);
        }
        
        [Fact]
        public void drone_should_have_start_in_route()
        {
            var mockedDispatcher = new Mock<IDispatcherGateway>().Object;
            var home = new GeoLocation{
                Latitude = 0.0m,
                Longitude = 0.0m
                
            };
            var dest = new GeoLocation{
                Latitude = 3.0m,
                Longitude = 4.0m
            };
            var drone = new DroneSimulator.Drone(Constants.DroneId, Constants.HomeLocation, mockedDispatcher, 1, Constants.DroneIp, Constants.Url);
            drone.Destination = dest;

            var route =  drone.GetRoute();
            route.Should().NotBeNull();
            route.Should().Contain(home);
        }
        
        [Fact]
        public void TestGetRouteAllPositiveNumbers()
        {
            var mockedDispatcher = new Mock<IDispatcherGateway>().Object;
            var home = new GeoLocation{
                Latitude = 0.0m,
                Longitude = 0.0m
                
            };
            var dest = new GeoLocation{
                Latitude = 3.0m,
                Longitude = 4.0m
            };
            var drone = new DroneSimulator.Drone(Constants.DroneId, Constants.HomeLocation, mockedDispatcher, 1, Constants.DroneIp, Constants.Url);
            drone.Destination = dest;
           
            var route =  drone.GetRoute();
            route.Should().NotBeNull();
            foreach (var geoLocation in route)
            {
                geoLocation.Latitude.Should().BeGreaterThanOrEqualTo(0m);
                geoLocation.Longitude.Should().BeGreaterThanOrEqualTo(0m);
            }
        }

        [Fact]
        public void TestGetRouteAllNegativeNumbers()
        {
            var mockedDispatcher = new Mock<IDispatcherGateway>().Object;
            var home = new GeoLocation{
                Latitude = 0.0m,
                Longitude = 0.0m
                
            };
            var dest = new GeoLocation{
                Latitude = -3.0m,
                Longitude = -4.0m
            };
            var drone = new DroneSimulator.Drone(Constants.DroneId, Constants.HomeLocation, mockedDispatcher, 1, Constants.DroneIp, Constants.Url);
            drone.Destination = dest;

            var route =  drone.GetRoute();
            route.Should().NotBeNull();
            foreach (var geoLocation in route)
            {
                geoLocation.Latitude.Should().BeLessThanOrEqualTo(0.0m);
                geoLocation.Longitude.Should().BeLessThanOrEqualTo(0.0m);
            }
        }
    }
}
*/
