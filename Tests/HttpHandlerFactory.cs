using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using Moq;
using Moq.Protected;
using SimDrone;

namespace Tests;

public class HttpHandlerFactory
{
    private Mock<HttpMessageHandler> _handler;


    public HttpHandlerFactory()
    {
        _handler = new Mock<HttpMessageHandler>();
    }

    public void newHttpHandlerSetup()
    {
        _handler = new Mock<HttpMessageHandler>();
    }
    public HttpMessageHandler GetHttpMessageHandler()
    {
        return _handler.Object;
    }
    public void SetupHttpMethod(string targetURIString, Func<object,Task<OkObjectResult>> destinationFunc, object dto)
    {
        var setup = new Mock<HttpMessageHandler>();
        setup.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync", 
                ItExpr.Is<HttpRequestMessage>(
                    x => x.RequestUri == 
                         new Uri(targetURIString)), 
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(destinationFunc(dto).IsCompletedSuccessfully.ToString())
            });
    }
    
    public void SetupHttpMethod(string targetURIString, Func<Drone,Task<OkObjectResult>> destinationFunc, Drone entity)
    {
        var setup = new Mock<HttpMessageHandler>();
        setup.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync", 
                ItExpr.Is<HttpRequestMessage>(
                    x => x.RequestUri == 
                         new Uri(targetURIString)), 
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(destinationFunc(entity).IsCompletedSuccessfully.ToString())
            });
    }
    public void OkAll()
    {
        var setup = new Mock<HttpMessageHandler>();
        setup.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync", 
                ItExpr.IsAny<HttpRequestMessage>(), 
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("")
            });
    }
    public void StringAll()
    {
        var setup = new Mock<HttpMessageHandler>();
        setup.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync", 
                ItExpr.IsAny<HttpRequestMessage>(), 
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("this is a string")
            });
    }
    public void jsonAll(Object o)
    {
        var setup = new Mock<HttpMessageHandler>();
        setup.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync", 
                ItExpr.IsAny<HttpRequestMessage>(), 
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(o.ToJson())
            });
    }
    
}