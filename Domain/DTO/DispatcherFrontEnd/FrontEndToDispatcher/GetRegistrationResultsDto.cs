using System;
using System.Net.Http;

namespace Domain.DTO.DispatcherFrontEnd.FrontEndToDispatcher{
    public class GetRegistrationResultsDto
    {
        public string IpAddress { get; set; }
        public Guid BadgeNumber { get; set; }
        
        public static GetRegistrationResultsDto From(HttpContent responseContent)
        {
            throw new NotImplementedException();
        }
    }
}
