using System;
using System.Net.Http;

namespace Domain.DTO.DispatcherFrontEnd.DispatcherToFrontEnd
{
    public class ResponseRegistrationDto
    {
        public bool Success { get; set; }

        public Guid BadgeNumber { get; set; }

        public static ResponseRegistrationDto From(HttpContent responseContent)
        {
            throw new NotImplementedException();
        }
    }
}