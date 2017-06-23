using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
namespace APIXULib
{
    public interface IRepository
    {
        WeatherModel GetWeatherData(string key, GetBy getBy, string value, Days ForecastOfDays );
        WeatherModel GetWeatherDataByLatLong( string key, string latitude, string longitude, Days ForecastOfDays);
        WeatherModel GetWeatherDataByAutoIP(string key, Days ForecastOfDays);

        WeatherModel GetWeatherData(string key, GetBy getBy, string value);
        WeatherModel GetWeatherDataByLatLong( string key, string latitude, string longitude);
        WeatherModel GetWeatherDataByAutoIP(string key);



    }
    public class Repository : IRepository
    {
        private string APIURL = "http://api.apixu.com/v1";

        #region Get Weather Forecast Data
        public WeatherModel GetWeatherData( string key, GetBy getBy, string value, Days ForecastOfDays)
        {
            return GetData(APIURL +RequestBuilder.PrepareRequest(MethodType.Forecast, key, getBy, value, ForecastOfDays));
        }
      

        public WeatherModel GetWeatherDataByLatLong( string key, string latitude, string longitude, Days ForecastOfDays)
        {
            return GetData(APIURL + RequestBuilder.PrepareRequestByLatLong(MethodType.Forecast, key,latitude,longitude, ForecastOfDays));

        }

        public WeatherModel GetWeatherDataByAutoIP( string key, Days ForecastOfDays)
        {
            return GetData(APIURL + RequestBuilder.PrepareRequestByAutoIP(MethodType.Forecast, key, ForecastOfDays));

        }
        #endregion

        #region Get Weather Current Data

        public WeatherModel GetWeatherData(string key, GetBy getBy, string value )
        {
            return GetData(APIURL + RequestBuilder.PrepareRequest(MethodType.Current, key, getBy, value));
        }


        public WeatherModel GetWeatherDataByLatLong(string key, string latitude, string longitude)
        {
            return GetData(APIURL + RequestBuilder.PrepareRequestByLatLong(MethodType.Current, key, latitude, longitude));

        }

        public WeatherModel GetWeatherDataByAutoIP(string key)
        {
            return GetData(APIURL + RequestBuilder.PrepareRequestByAutoIP(MethodType.Current, key));

        }

        #endregion

        private WeatherModel GetData(string url)
        {
            string urlParameters = "";
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri(url);

            // Add an Accept header for JSON format.
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));

            // List data response.
            HttpResponseMessage response = client.GetAsync(urlParameters).Result;  // Blocking call!
            if (response.IsSuccessStatusCode)
            {
                // Parse the response body. Blocking!
                return response.Content.ReadAsAsync<WeatherModel>().Result;
               
            }
            else
            {
                return new WeatherModel();
            }
        }
    }
}
