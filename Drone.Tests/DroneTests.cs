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

namespace Drone.Tests
{
    public class DroneTests
    {
        RestDbSvc restPoint = new RestDbSvc();
        private async Task<DroneModel> getRestDrone()
        {
            // Todo: register drone if not in the DB this creates duplicates
            var droneDeserialized = await restPoint.Get<DroneModel>("http://localhost:8080/Fleet/61fd97f2efe16a6bb253cbb7"); ;
            Console.WriteLine(droneDeserialized);
            return droneDeserialized;
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
                new (-3.0, sc-4.0)
            };
            Assert.Equal(expectedRoute, drone.GetRoute());
        }

        [Fact]
        public void TestDatabaseHasDrone()
        {
            // TO RUN: use ssh -L 8080:localhost:8080 root@147.182.227.239 using password given by Kamren

            var home = new Point(0.0, 0.0);

            var dest = new Point(-3.0, -4.0);

            var drone = new FlyingPizza.Drone.DroneModel(1, home);
            
            var resultDroneModel = getRestDrone().Result;
            // TODO: when put in async private task or waited, Serializer still chokes on drone object JSON
            
            Assert.Equal(drone, resultDroneModel);
        }
    }
}
