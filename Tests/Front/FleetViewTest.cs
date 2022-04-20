using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bunit;
using Xunit;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Radzen;
using Radzen.Blazor;

namespace Tests.Front;
[TestClass]

public class FleetViewTest
{
    [Fact]
    public static void TestFleetDrone()
    {
        //using var cxt = new Bunit.TestContext();
        //var check = cxt.RenderComponent<FrontEnd.Pages.FleetPages.FleetView>();
        //Xunit.Assert.Equal("Fleet Size: 2", check.Find($"b").TextContent);

        //check.Find($".rz-button").Click();
        //check.Find("b").MarkupMatches("<b>DroneId:Currentlocation:Domain.DTO.Shared.GeoLocationDestination: Domain.DTO.Shared.GeoLocationStatus: Charging</b>");

        
    }

}
