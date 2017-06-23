namespace Microsoft.Cognitive.LUIS.ActionBinding
{
    public class BuiltInGeographyTypes
    {
        public const string City = "builtin.geography.city";

        public const string Country = "builtin.geography.country";

        public const string PointOfInterest = "builtin.geography.pointOfInterest";

        public string CityType
        {
            get { return City; }
        }

        public string CountryType
        {
            get { return Country; }
        }

        public string PointOfInterestType
        {
            get { return PointOfInterest; }
        }
    }
}
