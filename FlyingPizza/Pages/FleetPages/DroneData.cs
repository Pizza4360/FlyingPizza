using FlyingPizza;

namespace FlyingDrone.Pages.FleetPages
{
    public class DroneData
    {

        public string DroneID { get; set; }
        public string Status { get; set; }
        public string Url { get; set; }
        public string Order { get; set; }
        public string batteryLife { get; set; }
        public GeoLocation location { get;set;}
        public GeoLocation destination { get; set; }

    }
}
