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

public class OrderPageTest
{
    [Fact]
    public static void TestOrderPage()
    {
        using var cxt = new Bunit.TestContext();
        var check = cxt.RenderComponent<FrontEnd.Pages.OrderPages.OrderPage>();

    }

}
