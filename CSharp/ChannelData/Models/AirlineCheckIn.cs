namespace ChannelDataBot.Models
{
    using Newtonsoft.Json;

    public class AirlineCheckIn
    {
        public AirlineCheckIn()
        {
            this.TemplateType = "airline_checkin";
        }

        [JsonProperty("template_type")]
        public string TemplateType { get; set; }

        [JsonProperty("intro_message")]
        public string IntroMessage { get; set; }

        [JsonProperty("locale")]
        public string Locale { get; set; }

        [JsonProperty("pnr_number")]
        public string PnrNumber { get; set; }

        [JsonProperty("flight_info")]
        public FlightInfo[] FlightInfo { get; set; }

        [JsonProperty("checkin_url")]
        public string CheckInUrl { get; set; }

        public override string ToString()
        {
            return $"{this.IntroMessage}. Confirmation Number: {this.PnrNumber}. Flight {this.FlightInfo[0].FlightNumber} from {this.FlightInfo[0].DepartureAirport.City} ({this.FlightInfo[0].DepartureAirport.AirportCode}) to {this.FlightInfo[0].ArrivalAirport.City} to ({this.FlightInfo[0].ArrivalAirport.AirportCode}) departing at {this.FlightInfo[0].FlightSchedule.DepartureTime} from gate {this.FlightInfo[0].DepartureAirport.Gate} at terminal {this.FlightInfo[0].DepartureAirport.Terminal} and arriving at {this.FlightInfo[0].FlightSchedule.ArrivalTime} to gate {this.FlightInfo[0].ArrivalAirport.Gate} at terminal {this.FlightInfo[0].ArrivalAirport.Terminal}. Boarding time is {this.FlightInfo[0].FlightSchedule.BoardingTime}. Check in @ {this.CheckInUrl}";
        }
    }
}