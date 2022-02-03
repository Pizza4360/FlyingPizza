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
        public void TestDatabaseHasLocation()
        {
            // TO RUN: use ssh -L 8080:localhost:8080 root@147.182.227.239 using password given by Kamren

            var home = new Point(0.0, 0.0);

            var dest = new Point(-3.0, -4.0);

            var drone = new FlyingPizza.Drone.Drone(1, home);

            drone.DeliverOrder(dest);
            var getTask = restPoint.Get<Point>("http://localhost:8080/Fleet/");
            getTask.Wait();
            var resultLocation = getTask.Result;
            // Waiting for rest to come back
            
            Assert.True(getTask.IsCompletedSuccessfully, "Failed to successfully Get on database");
            Assert.Equal(drone.Location, resultLocation);
        }
        [Fact]
        public void TestDatabaseHasStatus()
        {
            // TO RUN: use ssh -L 8080:localhost:8080 root@147.182.227.239 using password given by Kamren

            var home = new Point(0.0, 0.0);

            var dest = new Point(-3.0, -4.0);

            var drone = new FlyingPizza.Drone.Drone(1, home);

            drone.DeliverOrder(dest);
            var getTask = restPoint.Get<string>("http://localhost:8080/Fleet/");
            getTask.Wait();
            var resultStatus = getTask.Result;
            // Waiting for rest to come back
            
            Assert.True(getTask.IsCompletedSuccessfully, "Failed to successfully Get on database");
            Assert.Equal(drone.Status.ToString(), resultStatus);
        }
    }
}
