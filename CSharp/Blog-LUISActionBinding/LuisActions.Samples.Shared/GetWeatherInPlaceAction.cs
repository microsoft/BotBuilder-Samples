namespace LuisActions.Samples
{
    using System;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;
    using Microsoft.Cognitive.LUIS.ActionBinding;
    using Models;
    using Newtonsoft.Json.Linq;

    [Serializable]
    [LuisActionBinding("WeatherInPlace", FriendlyName = "Get the Weather in a location")]
    public class GetWeatherInPlaceAction : GetDataFromPlaceBaseAction
    {
        // we have API key here for simple demo purposes - should be at your app config file
        private const string APIKEY = "fcfe74816dd241fca46160635171002";

        public override Task<object> FulfillAsync()
        {
            var request = WebRequest.Create($"http://api.apixu.com/v1/current.json?key={APIKEY}&q={this.Place}");
            var response = (HttpWebResponse)request.GetResponse();

            var result = default(WeatherInfo);
            using (var sr = new StreamReader(response.GetResponseStream()))
            {
                dynamic data = JObject.Parse(sr.ReadToEnd());
                if (data.error == null)
                {
                    result = new WeatherInfo
                    {
                        Condition = data.current.condition.text,
                        Country = data.location.country,
                        Humidity = Convert.ToInt32(data.current.humidity),
                        Location = data.location.name
                    };
                }

                return Task.FromResult((object)result);
            }
        }
    }
}