using System.Web;
using Domain.DTO;
using Newtonsoft.Json.Linq;

namespace DatabaseAccess.Services;


public static class LocationParser
{
    private const string BaseUrl = "https://maps.googleapis.com/maps/api";
    private static readonly HttpClient HttpClient = new();

    // Use the api key and address to form a full google maps location request url.
    public static async Task<GeoLocation> Parse(string apiKey, string endpoint)
    {
        Console.WriteLine($"\n\n\n\nParsing address from response:{endpoint}\n\n\n\n");
        var requestUrl = $"{BaseUrl}/geocode/json?address={HttpUtility.UrlEncode(endpoint)},&key={apiKey};
        var response = await HttpClient.GetStringAsync(requestUrl);
        Console.Write("RETURNED: " + response);
        var js = JObject.Parse(response);
        var Longitude = decimal.Parse(Convert.ToString(js["results"]?[0]?["geometry"]?["location"]?["lng"]) ?? string.Empty);
        var Latitude = decimal.Parse(Convert.ToString(js["results"]?[0]?["geometry"]?["location"]?["lat"]) ?? string.Empty);
        return new GeoLocation {Latitude, Longitude};
    }
}
