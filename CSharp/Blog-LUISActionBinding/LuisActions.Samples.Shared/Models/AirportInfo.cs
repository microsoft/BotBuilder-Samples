namespace LuisActions.Samples.Models
{
    public class AirportInfo
    {
        public string Code { get; set; }

        public string Name { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public string Location { get; set; }

        public override string ToString()
        {
            return $"CODE: {this.Code.ToUpperInvariant()} - Name: \"{this.Name}\" - City: {this.City} - Country: {this.Country} - Location: {this.Location}";
        }
    }
}