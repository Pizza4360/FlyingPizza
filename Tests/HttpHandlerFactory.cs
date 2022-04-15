using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Domain.DTO;
using Domain.Entities;
using Microsoft.AspNetCore.Mvc;
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
    public void SetupHttpMethod(string targetURIString, Func<BaseDTO,Task<OkObjectResult>> destinationFunc, BaseDTO dto)
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

    public void SetupHttpMethod(string targetURIString)
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
                Content = new StringContent("ok")
            });
    }
    public void OkAllHttp()
    {
        var setup = new Mock<HttpMessageHandler>();
        setup.Protected().Setup<Task<HttpResponseMessage>>(
                "SendAsync", 
                ItExpr.IsAny<HttpRequestMessage>(), 
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(new HttpResponseMessage {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent("ok")
            });
    }
}