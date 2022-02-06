using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using FlyingDrone;
using FlyingPizza.Drone;
using FlyingPizza.Services;
using Microsoft.AspNetCore.Components;
using Xunit;
using Xunit.Abstractions;
using System.Linq;

namespace Drone.Tests
{
    public class DroneTests
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public DroneTests(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
        }
        [Fact]
        public void TestGetRouteAllPositiveNumbers()
        {
            var home = new Point(0.0, 0.0);
            var dest = new Point(3.0, 4.0);

            var drone = new FlyingPizza.Drone.DroneModel(1, home);
            drone.Delivery = dest;

            Point[] expectedRoute = {
                new (0.6, 0.8),
                new (1.2, 1.6),
                new (1.8, 2.4),
                new (2.4, 3.2),
                new (3.0, 4.0)
            };
            Assert.Equal(expectedRoute, drone.GetRoute());
        }

        [Fact]
        public void TestGetRouteAllNegativeNumbers()
        {
            var home = new Point(0.0, 0.0);
            var dest = new Point(-3.0, -4.0);

            var drone = new FlyingPizza.Drone.DroneModel(1, home);
            drone.Delivery = dest;

            Point[] expectedRoute = {
                new (-0.6, -0.8),
                new (-1.2, -1.6),
                new (-1.8, -2.4),
                new (-2.4, -3.2),
                new (-3.0, -4.0)
            };
            Assert.Equal(expectedRoute, drone.GetRoute());
        }

        [Fact]
        public void TestDatabaseHasDrone()
        {
            // TO RUN: use ssh -L 8080:localhost:8080 root@147.182.227.239 using password given by Kamren

            var home = new Point(0.0, 0.0);

            var dest = new Point(-3.0, -4.0);

            var drone = new FlyingPizza.Drone.DroneModel(2, home);
            var resultDroneRecord = drone.readDbArray();
            var res  = resultDroneRecord.Result;

            
            
            Assert.Equal(drone, res[0]);
        }
    }
}
