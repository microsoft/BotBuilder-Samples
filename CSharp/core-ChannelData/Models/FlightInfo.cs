namespace ChannelDataBot.Models
{
    using Newtonsoft.Json;

    public class FlightInfo
    {
        [JsonProperty("flight_number")]
        public string FlightNumber { get; set; }

        [JsonProperty("departure_airport")]
        public Airport DepartureAirport { get; set; }

        [JsonProperty("arrival_airport")]
        public Airport ArrivalAirport { get; set; }

        [JsonProperty("flight_schedule")]
        public FlightSchedule FlightSchedule { get; set; }
    }
}