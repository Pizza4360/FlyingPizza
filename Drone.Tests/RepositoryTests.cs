using Xunit;
namespace Drone.Tests
{
    public class RepositoryTests
    {
        // Fixtures for quality of life

        // Assert fail
        private static void AssertFail(string msg)
        {
            Assert.True(false, msg);
        }

        // Tests
        [Fact]
        public void TestDronesGetDrone()
        {
            AssertFail("Not implemented");
        }
        [Fact]
        public void TestDronesGetAll()
        {
            AssertFail("Not implemented");
        }

        [Fact]
        public void TestMongoCreate()
        {
            AssertFail("Not implemented");
        }
        [Fact]
        public void TestMongoDelete()
        {
            AssertFail("Not implemented");
        }
        [Fact]
        public void TestMongoUpdate()
        {
            AssertFail("Not implemented");
        }
        [Fact]
        public void TestMongoGetById()
        {
            AssertFail("Not implemented");
        }
        [Fact]
        public void TestMongoGetWhereAll()
        {
            AssertFail("Not implemented");
        }
    }

}