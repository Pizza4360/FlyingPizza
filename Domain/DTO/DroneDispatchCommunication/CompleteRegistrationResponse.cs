﻿using Domain.Entities;

namespace Domain.DTO.DroneDispatchCommunication;

public class CompleteRegistrationResponse
{
    
    public DroneRecord Record { get; set; }
    public bool Okay { get; set; }
}