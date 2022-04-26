using System;
using System.Text.Json.Serialization;

namespace Domain.DTO.FrontEndDispatchCommunication
{
    public class AddDroneResponse
    {
        public bool Success { get; set; }
        public Guid BadgeNumber { get; set; }    
    }
}