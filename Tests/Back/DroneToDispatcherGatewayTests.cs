using System.Threading.Tasks;
using FluentAssertions;
using SimDrone;
using Xunit;

namespace Tests.Back.Unit
{
    public class DroneToDispatcherGatewayTests
    {
        // [Fact]
        // public async Task complete_delivery_should_return_dto()
        // {
        //     var handlerFactory = new HttpHandlerFactory();
        //     handlerFactory.jsonAll(Constants.TestCompleteDeliveryResponse);
        //     var handle = handlerFactory.GetHttpMessageHandler();
        //     var droneGateway = new DroneToDispatchGateway("unused");
        //     droneGateway.ChangeHandler(handle);
        //     var response = await droneGateway.CompleteDelivery(Constants.TestCompleteDeliveryRequest);
        //     response.Should().NotBeNull();
        //     response.Should().BeEquivalentTo(Constants.TestCompleteDeliveryResponse);
        // } Untestable, mocked httpclient hidden by real one from baseGateway
        
        
        // [Fact]
        // public async Task update_drone_status_should_return_dto()
        // {
        //     var handlerFactory = new HttpHandlerFactory();
        //     handlerFactory.jsonAll(Constants.TestUpdateResponse);
        //     var handle = handlerFactory.GetHttpMessageHandler();
        //     var droneGateway = new DroneToDispatchGateway("unused");
        //     droneGateway.ChangeHandler(handle);
        //     var response = await droneGateway.UpdateDroneStatus(Constants.TestUpdate);
        //     response.Should().NotBeNull();
        //     response.Should().BeEquivalentTo(Constants.TestUpdateResponse);
        // } Untestable, mocked httpclient hidden by real one from baseGateway
        
    }
}