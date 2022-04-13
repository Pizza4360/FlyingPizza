using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Domain.DTO
{
    public enum DroneState
    {
        Ready,
        Delivering,
        Returning,
        Dead,
        Charging
    }
}