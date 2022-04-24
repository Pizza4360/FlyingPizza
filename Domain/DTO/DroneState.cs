using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Domain.DTO.DroneDispatchCommunication;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.IdGenerators;

namespace Domain.DTO;

public enum DroneState
{
    Ready,
    Delivering,
    Returning,
    Dead,
    Charging
}
public static class DroneStateExtensions
{
    private static readonly Dictionary<DroneState, string> StateToColorDict = new(){
        { DroneState.Ready, "#08af08" },
        { DroneState.Delivering, "#98f5ff" },
        { DroneState.Returning, "#8A2BE2" },
        { DroneState.Dead, "#FC401E" },
        { DroneState.Charging, "#FF9A08" }
    };
    private static readonly Dictionary<DroneState, string> StateToStringDict = new(){
        { DroneState.Ready, "Ready" },
        { DroneState.Delivering, "Delivering" },
        { DroneState.Returning, "Returning" },
        { DroneState.Dead, "Dead" },
        { DroneState.Charging, "Charging" }
    };
    public static string GetColor(this DroneState state) => StateToColorDict[state];
    public static string ToString(this DroneState state) => StateToStringDict[state];
}
