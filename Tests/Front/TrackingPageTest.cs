using Microsoft.VisualStudio.TestTools.UnitTesting;
using Xunit;
using TestContext = Bunit.TestContext;

namespace Tests.Front;

[TestClass]
public class TrackingPageTest
{
    [Fact]
    public static void TestTrackingPage()
    {
        using var cxt = new TestContext();
    }
}