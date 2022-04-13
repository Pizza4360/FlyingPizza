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
using Microsoft.Extensions.DependencyInjection;
using FrontEnd.Services;
using FrontEnd.Pages.OrderPages;

namespace Tests.Front;
[TestClass]

public class OrderPageTest
{
    [Fact]
    public static void TestOrderPage()
    {
        using var cxt = new Bunit.TestContext();
        //var respoint = cxt.Services.AddSingleton(new RestDbSvc());
        var check = cxt.RenderComponent<OrderPage>();

    }
}
