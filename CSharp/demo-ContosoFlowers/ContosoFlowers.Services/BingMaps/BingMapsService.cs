namespace ContosoFlowers.Services.BingMaps
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using System.Web;
    using Newtonsoft.Json;

    public class BingMapsService
    {
        private const string LocationServiceUriTemplate = "https://dev.virtualearth.net/REST/v1/Locations?key={0}&q={1}";

        private readonly string apiKey;

        public BingMapsService(string apiKey)
        {
            if (string.IsNullOrWhiteSpace(apiKey))
            {
                throw new ArgumentNullException("apiKey", "Bing Maps Key is required.");
            }

            this.apiKey = apiKey;
        }

        public async Task<IEnumerable<Address>> GeoCode(string inputAddress)
        {
            Uri geocodeUri = new Uri(string.Format(LocationServiceUriTemplate, this.apiKey, HttpUtility.UrlEncode(inputAddress)));
            var result = await GetResponse(geocodeUri);

            if (result.ResourceSets.Any())
            {
                return result.ResourceSets[0].Resources
                    .Where(o => o.GetType() == typeof(Location)).Select(o => o as Location)
                    .Where(a => a.EntityType == "Address")
                    .Select(a => a.Address);
            }

            return Enumerable.Empty<Address>();
        }

        private static async Task<Response> GetResponse(Uri uri)
        {
            using (var client = new WebClient() { Encoding = Encoding.UTF8 })
            {
                var json = await client.DownloadStringTaskAsync(uri);
                return JsonConvert.DeserializeObject<Response>(json, new BingDataConverter());
            }
        }
    }
}
