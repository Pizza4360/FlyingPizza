using Xunit;
namespace Drone.Tests
{
    public class DispatcherTests
    {
        // Fixtures for quality of life
    
        // Assert fail
        private static void AssertFail(string msg)
        {
            Assert.True(false, msg);
        }
        
        // Tests
        [Fact]
        public void TestRegister()
        {
            AssertFail("Not implemented");
        }
        [Fact]
        public void TestAddOrder()
        {
            AssertFail("Not implemented");
        }
        [Fact]
        public void TestCompleteDelivery()
        {
            AssertFail("Not implemented");
        }
        [Fact]
        public void TestReadyForOrder()
        {
            AssertFail("Not implemented");
        }
    }
}