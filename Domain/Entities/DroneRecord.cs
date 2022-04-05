using System;
using Domain.Interfaces;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities;

public class DroneRecord: IBaseEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }

    public Guid BadgeNumber { get; set; }
    
    public GeoLocation CurrentLocation { get; set; }
        
    public GeoLocation Destination { get; set; }

    public string OrderId { get; set; }

    public DroneState State { get; set; }
        
    public GeoLocation HomeLocation { get; set; }
        
    public string IpAddress { get; set; }

    public string DispatcherUrl { get; set; }
    
    public override bool Equals(object o)
    {
        if (o == null || o.GetType() != GetType()) return false;
        DroneRecord oo = (DroneRecord) o;
        return oo.BadgeNumber == BadgeNumber &&
               oo.CurrentLocation.Equals(CurrentLocation) &&
               oo.Destination.Equals(Destination) &&
               oo.State.Equals(State);
    }
    
    // String for debugging GetDronepurposes
    public override string ToString()
    {
        return $"ID:{BadgeNumber}\n" +
               $"location:{CurrentLocation}\n" +
               $"Destination:{Destination}\n" +
               $"Status:{State}";
    }
}