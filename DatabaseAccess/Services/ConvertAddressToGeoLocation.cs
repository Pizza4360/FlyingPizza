using System.Web;
using Domain.DTO;
using Newtonsoft.Json.Linq;

namespace DatabaseAccess.Services;

public static class LocationParser
{
    private const string UrlStart = "https://maps.googleapis.com/maps/api/";
    private static readonly HttpClient HttpClient = new();

    public static async Task<GeoLocation> Parse(string apiKey, string address)
    {
        Console.WriteLine($"\n\n\n\nParsing address from response:{address}\n\n\n\n");
        var url = UrlStart + "geocode/json?address=" + HttpUtility.UrlEncode(address) + ",&key=" + apiKey;
        var req = await HttpClient.GetStringAsync(url);
        Console.Write("RETURNED: " + req);
        var js = JObject.Parse(req);
        var lng = decimal.Parse(Convert.ToString(js["results"]?[0]?["geometry"]?["location"]?["lng"]) ?? string.Empty);
        var lat = decimal.Parse(Convert.ToString(js["results"]?[0]?["geometry"]?["location"]?["lat"]) ?? string.Empty);
        return new GeoLocation {Latitude = lat, Longitude = lng};
    }
}