using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Domain.DTO;
using Newtonsoft.Json.Linq;

namespace Domain;

public class ConvertAddressToGeoLocation
{
    private const string _urlStart = "https://maps.googleapis.com/maps/api/";
    private readonly HttpClient httpClient = new();

    public ConvertAddressToGeoLocation()
    {
        ApiKey = "AIzaSyABM05Ov28GgnvCE6fvNUT0hmPB7Ol6kuI";
    }

    public string ApiKey { get; set; }

    //  "MapsApiKey": "https://maps.googleapis.com/maps/api/geocode/json?address=1600+Est%C3%A2ncia+Sergipe,&key=AIzaSyABM05Ov28GgnvCE6fvNUT0hmPB7Ol6kuI",

    public async Task<GeoLocation> CoordsFromAddress(string address)
    {
        var url = _urlStart + "geocode/json?address=" + HttpUtility.UrlEncode(address) + ",&key=" + ApiKey;
        var req = await httpClient.GetStringAsync(url);

        Console.Write("RETURNED FROM RED: " + req);

        var js = JObject.Parse(req);
        var lng = decimal.Parse(Convert.ToString(js["results"]?[0]?["geometry"]?["location"]?["lng"]) ?? string.Empty);
        var lat = decimal.Parse(Convert.ToString(js["results"]?[0]?["geometry"]?["location"]?["lat"]) ?? string.Empty);
        return new GeoLocation {Latitude = lat, Longitude = lng};
    }
}