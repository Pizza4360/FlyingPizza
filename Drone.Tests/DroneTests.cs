using FlyingDrone;
using FlyingPizza.Drone;
using Xunit;

namespace Drone.Tests
{
    public class DroneTests
    {
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
            //TODO: I have no idea how to hook to this dang thing by C# since
            // Apparently I can't understand webdev sorry
            Assert.False(true, "Not implemented");
        }
        [Fact]
        public void TestDatabaseHasStatus()
        {

            //TODO: I have no idea how to hook to this dang thing by C#
            Assert.False(true, "Not implemented");
        }
    }
}
