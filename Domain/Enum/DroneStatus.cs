using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Domain.DTO.DroneDispatchCommunication;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Domain.DTO;

public enum DroneStatus
{
    Ready,
    Delivering,
    Returning,
    Disconnected,
    Charging,
    Unititialized,
    Assigned
}
public static class DroneStatusExtensions
{
    private static readonly Dictionary<DroneStatus, string> StatusToColorDict
        = new(){
            { DroneStatus.Ready, "#13FF00" },
            { DroneStatus.Delivering, "#98f5ff" },
            { DroneStatus.Returning, "#8A2BE2" },
            { DroneStatus.Disconnected, "#FF0000" },
            { DroneStatus.Charging, "#FF9A08" },
            { DroneStatus.Unititialized, "#964B00" },
            { DroneStatus.Assigned, "#FF9A08" }
        };
    private static readonly Dictionary<DroneStatus, string> StatusToStringDict 
        = new(){
            { DroneStatus.Ready, "Ready" },
            { DroneStatus.Delivering, "Delivering" },
            { DroneStatus.Returning, "Returning" },
            { DroneStatus.Disconnected, "Dead" },
            { DroneStatus.Unititialized, "Unititialized" },
            { DroneStatus.Charging, "Charging" },
            { DroneStatus.Assigned, "Assigned" }
        };
    
    public static string GetColor(this DroneStatus status)
        => StatusToColorDict[status];
    
    public static string ToString(this DroneStatus status)
        => StatusToStringDict[status];
}
