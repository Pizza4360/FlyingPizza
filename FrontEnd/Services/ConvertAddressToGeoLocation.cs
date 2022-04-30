using System;
using System.Net.Http;
using System.Threading.Tasks;
using Domain.DTO;
using Newtonsoft.Json.Linq;

namespace FrontEnd.Services
{
    public class ConvertAddressToGeoLocation
    {
        private string ApiKey { get; set; }
        private const string UrlStart = "https://maps.googleapis.com/maps/api/";
        private HttpClient httpClient = new HttpClient();

         public ConvertAddressToGeoLocation(string apiKey) {
             ApiKey = apiKey;
         }
         
        public async Task<GeoLocation> CoordsFromAddress(string address)
        {
            var url = UrlStart + "geocode/json?address=" + System.Web.HttpUtility.UrlEncode(address) + ",&key=" + ApiKey;
            var req = await httpClient.GetStringAsync(url);

            Console.Write("RETURNED FROM RED: " + req);
      
            var js = JObject.Parse(req);
            var lng = decimal.Parse(Convert.ToString(js["results"]?[0]?["geometry"]?["location"]?["lng"]) ?? string.Empty);
            var lat = decimal.Parse(Convert.ToString(js["results"]?[0]?["geometry"]?["location"]?["lat"]) ?? string.Empty);
            return new GeoLocation { Latitude = lat, Longitude = lng};
        }

    }
}
