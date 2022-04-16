using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace FrontEnd.Services
{

    // this service is called by any and all methods and classes that need to make a request to the rest database

    public static class HttpMethods
    {
        // create http client initialized null
        private static HttpClient http = new();


        // this method submits a put request taking in an object of type T and returning an http response containing
        // the associated information
        public static async Task<HttpResponseMessage> Put<T>(string url, T item)
        {
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(
                    System.Text.Encoding.ASCII.GetBytes("admin:secret")));


            http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var r = await http.PutAsJsonAsync(url, item);
            string str = await r.Content.ReadAsStringAsync();

            return r;
        }

        // this method submits a post request taking in an object of type T and returning an http response containing
        // the associated information
        public static async Task<HttpResponseMessage> Post<T>(string url, T item)
        {
            if (http == null) http = new HttpClient();

            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                "Basic",
                Convert.ToBase64String(
                    System.Text.Encoding.ASCII.GetBytes("admin:secret")));

            http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var r = await http.PostAsJsonAsync(url, item);
            string str = await r.Content.ReadAsStringAsync();

            return r;
        }

        public static async Task<HttpResponseMessage> Patch<T>(string url, T item)
        {
            using (var client = new HttpClient())
            {
                var request = new HttpRequestMessage(new HttpMethod("PATCH"), url);

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(
                    "Basic",
                    Convert.ToBase64String(
                        System.Text.Encoding.ASCII.GetBytes("admin:secret")));


                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                
                // deserialize item the json
                string json = JsonSerializer.Serialize<T>(item);

                StringContent s = new StringContent(json, Encoding.UTF8, "application/json");

                request.Content = s;

                var response = await client.SendAsync(request);

                string str = await response.Content.ReadAsStringAsync();

                return response;
            }
        }


        // this method submits a get request taking in an object of type T and returning an http response containing
        // the associated information
        public static async Task<T> Get<T>(string url)
        {
            if (http == null) http = new HttpClient();
            var r = await http.GetAsync(url);

            // auto logout on 401 response
            if (r.StatusCode == System.Net.HttpStatusCode.Unauthorized) return default(T);

            // throw exception on error response
            if (!r.IsSuccessStatusCode)
            {
                var error = await r.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                return default(T);
            }

            return await r.Content.ReadFromJsonAsync<T>();
        }
    }
}
