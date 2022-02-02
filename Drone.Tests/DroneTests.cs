using System;
using FlyingDrone;
using FlyingPizza.Drone;
using FlyingPizza.Services;
using Xunit;

namespace Drone.Tests
{
    public class DroneTests
    {
        RestDbSvc restPoint = new RestDbSvc();
        
        [Fact]
        public void TestGetRouteAllPositiveNumbers()
        {
            var home = new Point(0.0, 0.0);
            var dest = new Point(3.0, 4.0);

            var drone = new FlyingPizza.Drone.Drone(1, home);
            drone.Destination = dest;

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

            var drone = new FlyingPizza.Drone.Drone(1, home);
            drone.Destination = dest;

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
        public void TestDatabaseHasFields()
        {
            // TODO: this GET works, but POST fails
            // TO RUN: use ssh -L 8080:localhost:8080 root@147.182.227.239 using password given by Kamren

            var home = new Point(0.0, 0.0);

            var dest = new Point(-3.0, -4.0);

            var drone = new FlyingPizza.Drone.Drone(1, home);

            drone.DeliverOrder(dest);
            var getTask = restPoint.Get<FlyingPizza.Drone.Drone>("http://localhost:8080/" + drone.Id);
            getTask.Wait();
            // Waiting for rest to come back

            FlyingPizza.Drone.Drone savedDrone = getTask.Result;
            Assert.True(getTask.IsCompletedSuccessfully, "Failed to successfully Get on database");
            Assert.Equal(drone, savedDrone);
        }
    }
}
