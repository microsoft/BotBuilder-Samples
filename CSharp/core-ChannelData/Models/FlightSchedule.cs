namespace ChannelDataBot.Models
{
    using System;
    using Newtonsoft.Json;

    public class FlightSchedule
    {
        [JsonProperty("boarding_time")]
        public string BoardingTime { get; set; }

        [JsonProperty("departure_time")]
        public string DepartureTime { get; set; }

        [JsonProperty("arrival_time")]
        public string ArrivalTime { get; set; }
    }
}