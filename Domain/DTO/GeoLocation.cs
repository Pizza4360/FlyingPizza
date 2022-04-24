using System;
using System.Text.Json.Serialization;
using Domain.Entities;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.DTO
{
    public class GeoLocation
    {
        private const decimal Tolerance = 0.01m;

        [BsonElement("Latitude")]
        [JsonPropertyName("Latitude")]
        [BsonRepresentation(BsonType.String)]
        public decimal Latitude { get; set; }

        [BsonElement("Longitude")]
        [JsonPropertyName("Longitude")]
        [BsonRepresentation(BsonType.String)]
        public decimal Longitude { get; set; }

        public override bool Equals(object? o)
        {
            return o is GeoLocation other &&
                   Math.Abs(Latitude - other.Latitude) < Tolerance
                   && Math.Abs(Longitude - other.Longitude) < Tolerance;
        }


        public override string ToString()
        {
            return $"{{Latitude:{Latitude},Longitude:{Longitude}}}";
        }


        public override int GetHashCode() => HashCode.Combine(Latitude, Longitude);


        public GeoLocation OffSet(GeoLocation routeLocation) => new()
        {
            Latitude=routeLocation.Latitude + Latitude,
            Longitude = routeLocation.Longitude + Longitude
        };


        public bool IsBetween(GeoLocation location1, GeoLocation location2) =>
            (location1.Longitude >= Longitude && Longitude >= location2.Longitude ||
             location2.Longitude >= Longitude && Longitude >= location1.Longitude) &&
            (location1.Latitude >= Latitude && Latitude >= location2.Latitude ||
             location2.Latitude >= Latitude && Latitude >= location1.Latitude);
    }
}
