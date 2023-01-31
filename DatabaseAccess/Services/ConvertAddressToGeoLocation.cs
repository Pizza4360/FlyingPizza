using System.Web;
using Domain.DTO;
using Newtonsoft.Json.Linq;

namespace DatabaseAccess.Services;


/// <summary>
/// This service is used by the DatabaseAccess layer, since that is the object
/// which puts new deliveries in the database.
/// LocationParser will convert a street address into Latitude/Longitude before
/// inserting the delivery into a database.
/// </summary>
public static class LocationParser
{
    private const string BaseUrl = "https://maps.googleapis.com/maps/api";
    private static readonly HttpClient HttpClient = new();

    /// <summary>
    ///  Use the api key and address to form a full google maps location request
    /// url.
    /// </summary>
    /// <param name="apiKey">a valid google api key to request the conversion
    /// </param>
    /// <param name="address">the street address to be converted.</param>
    /// <returns></returns>
    public static async Task<GeoLocation> Parse(string apiKey, string address)
    {
        Console.WriteLine(
            $"\n\n\n\n"
            + $"Parsing address from response:{address}"
            + $"\n\n\n\n");
        
        var requestUrl
            = $"{BaseUrl}/geocode/json"
              + $"?address={HttpUtility.UrlEncode(address)}"
              + $",&key={apiKey}";
        
        var response = await HttpClient.GetStringAsync(requestUrl);
        Console.Write("RETURNED: " + response);
        
        var js = JObject.Parse(response);
        var jToken = js["results"]?[0]?["geometry"]?["location"];
        return new GeoLocation
        {
            Latitude =  ExtractFromJToken(jToken, "lat"), 
            Longitude = ExtractFromJToken(jToken, "lng"), 
        };
    }
    
    /// <summary>
    /// Helper method to simplify the code. Parses a double
    /// from either the response's latitude or longitude.
    /// </summary>
    /// <param name="location">the part of the response containing "lat"
    /// and "lng"</param>
    /// <param name="latitudeOrLongitude">either "lat" or "lng"</param>
    /// <returns>the parsed double</returns>
    private static decimal ExtractFromJToken(
        JToken? location,
        string latitudeOrLongitude
    ) => decimal.Parse(
            Convert.ToString(location?[latitudeOrLongitude]) ?? string.Empty
        );
}
