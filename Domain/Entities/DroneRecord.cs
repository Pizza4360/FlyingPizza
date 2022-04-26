using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using Domain.DTO;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

public class DroneRecord
{
    [BsonId] 
    public string DroneId{get;set;}
    public List<Order> Orders{get;set;}
    public Guid BadgeNumber { get; set; }
    public GeoLocation Destination { get; set; }
    public GeoLocation CurrentLocation { get; set; }
    public GeoLocation HomeLocation { get; set; }
    public DroneState State { get; set; }
    public string DroneUrl { get; set; }
    public string DispatchUrl { get; set; }
    public override string ToString()
    {
        return $"Orders:{Orders}" +
               $"Currentlocation:{CurrentLocation}\n" +
               $"Destination:{Destination}\n" +
               $"Status:{State}";
    }
    public override bool Equals(object o)
    {
        if (o == null ||
            o.GetType() != GetType()) return false;
        DroneRecord oo = (DroneRecord) o;
        return oo.BadgeNumber == BadgeNumber &&
               oo.CurrentLocation.Equals(CurrentLocation) &&
               oo.Destination.Equals(Destination) &&
               oo.State.Equals(State) &&
               oo.DispatchUrl.Equals(DispatchUrl);
    }
}