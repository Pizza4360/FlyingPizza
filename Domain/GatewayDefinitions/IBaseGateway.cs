using System.Collections.Generic;
using System.Threading.Tasks;
using Domain.DTO.FrontEndDispatchCommunication;
using Domain.DTO.SchedulerDispatch;

namespace Domain.GatewayDefinitions;

public interface IBaseGateway<T1>
{
    public Task<TResponse> SendMessagePost<TRequest, TResponse>(string url, TRequest requestDto,
        bool isDebug = true);
}