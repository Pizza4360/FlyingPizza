using Domain.Entities;

namespace Tests
{
    public class Constants
    {
        public const string DroneIp = "localhost:5000";
        public const string DispatcherIp = "localhost:4000";
        public const string Url = "https://FlyingPizza.com";
        public const int BadgeNumber = 1;
        public const string DroneId = "abcdefg";
        public static readonly GeoLocation HomeLocation = new()
        {
            Latitude = 0.00001m,
            Longitude = 0.00001m
        };
        public static readonly GeoLocation Destination = new()
        {
            Latitude = 0.00008m,
            Longitude = 0.00001m
        };
        public const string CHARGING = "Charging";
    }
}
        

