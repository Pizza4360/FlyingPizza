using FlyingPizza.Services;
using Microsoft.AspNetCore.Components;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;


namespace FlyingPizza.Domain.Entities
{
    public class DroneModel : ComponentBase
    {
        // Connection to the database
        private RestDbSvc RestSvc { get; }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        
        // The unique ID of this drone which is stored in the database
        public int BadgeNumber { get; set; }
        
        // The current position of the drone
        public GeoLocation CurrentLocation { get; set; } 
        
        // The desired position of the drone
        public GeoLocation Destination { get; set; }
        // Current status of the drone
        public string State { get; set; }

        // The url to send commands to a drone
        public GeoLocation HomeLocation { get; set; }
        
        public string IpAddress { get; set; }

        
        // String for debugging GetDronepurposes
        public override string ToString()
        {
            return $"ID:{BadgeNumber}\n" +
                   $"location:{CurrentLocation}\n" +
                   $"Destination:{Destination}\n" +
                   $"Status:{State}";
        }

        public override bool Equals(object o)
        {
            if (o == null || o.GetType() != GetType()) return false;
            DroneModel oo = (DroneModel) o;
            return oo.BadgeNumber == BadgeNumber &&
                   oo.CurrentLocation.Equals(CurrentLocation) &&
                   oo.Destination.Equals(Destination) &&
                   oo.State.Equals(State);
        }
    }
}
