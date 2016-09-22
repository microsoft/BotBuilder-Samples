namespace ChannelDataBot.Models
{
    using Newtonsoft.Json;

    public class Airport
    {
        [JsonProperty("airport_code")]
        public string AirportCode { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("terminal")]
        public string Terminal { get; set; }

        [JsonProperty("gate")]
        public string Gate { get; set; }
    }
}