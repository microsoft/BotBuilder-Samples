namespace LuisActions.Samples.Models
{
    public class WeatherInfo
    {
        public string Location { get; set; }

        public string Country { get; set; }

        public string Condition { get; set; }

        public int Humidity { get; set; }

        public override string ToString()
        {
            return $"Location: {this.Location}, {this.Country} - Condition: {this.Condition} - Humidity: {this.Humidity}%";
        }
    }
}