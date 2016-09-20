namespace ContosoFlowers.Services.BingMaps
{
    using System;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    /// <summary>
    /// https://msdn.microsoft.com/en-us/library/jj870778.aspx
    /// </summary>
    public class Address
    {
        public string AddressLine { get; set; }

        public string AdminDistrict { get; set; }

        public string AdminDistrict2 { get; set; }

        public string CountryRegion { get; set; }

        public string CountryRegionIso2 { get; set; }

        public string FormattedAddress { get; set; }

        public string Locality { get; set; }

        public string PostalCode { get; set; }

        public string Neighborhood { get; set; }

        public string Landmark { get; set; }
    }

    public class BirdseyeMetadata : ImageryMetadata
    {
        public double Orientation { get; set; }

        public int TilesX { get; set; }

        public int TilesY { get; set; }
    }

    public class BoundingBox
    {
        public double SouthLatitude { get; set; }

        public double WestLongitude { get; set; }

        public double NorthLatitude { get; set; }

        public double EastLongitude { get; set; }
    }

    public class Detail
    {
        public int CompassDegrees { get; set; }

        public string ManeuverType { get; set; }

        public int[] StartPathIndices { get; set; }

        public int[] EndPathIndices { get; set; }

        public string RoadType { get; set; }

        public string[] LocationCodes { get; set; }

        public string[] Names { get; set; }

        public string Mode { get; set; }

        public RoadShield roadShieldRequestParameters { get; set; }
    }

    public class Generalization
    {
        public int[] PathIndices { get; set; }

        public double LatLongTolerance { get; set; }
    }

    public class Hint
    {
        public string HintType { get; set; }

        public string Text { get; set; }
    }

    public class ImageryMetadata : Resource
    {
        public string ImageHeight { get; set; }

        public string ImageWidth { get; set; }

        public string ImageUrl { get; set; }

        public string[] ImageUrlSubdomains { get; set; }

        public string VintageEnd { get; set; }

        public string VintageStart { get; set; }

        public int ZoomMax { get; set; }

        public int ZoomMin { get; set; }
    }

    public class Instruction
    {
        public string ManeuverType { get; set; }

        public string Text { get; set; }
    }

    public class ItineraryItem
    {
        public ItineraryItem[] ChildItineraryItems { get; set; }

        public string CompassDirection { get; set; }

        public Detail[] Details { get; set; }

        public string Exit { get; set; }

        public Hint[] Hints { get; set; }

        public string IconType { get; set; }

        public Instruction Instruction { get; set; }

        public Point ManeuverPoint { get; set; }

        public string SideOfStreet { get; set; }

        public string[] Signs { get; set; }

        public string Time { get; set; }

        public string TollZone { get; set; }

        public string TowardsRoadName { get; set; }

        public TransitLine TransitLine { get; set; }

        public int TransitStopId { get; set; }

        public string TransitTerminus { get; set; }

        public double TravelDistance { get; set; }

        public double TravelDuration { get; set; }

        public string TravelMode { get; set; }

        public Warning[] Warning { get; set; }
    }

    public class Line
    {
        public string Type { get; set; }

        public double[][] Coordinates { get; set; }
    }

    public class Location : Resource
    {
        public string Name { get; set; }

        public Point Point { get; set; }

        public string EntityType { get; set; }

        public Address Address { get; set; }

        public string Confidence { get; set; }

        public string[] MatchCodes { get; set; }

        public Point[] GeocodePoints { get; set; }

        public QueryParseValue[] QueryParseValues { get; set; }
    }

    public class QueryParseValue
    {
        public string Property { get; set; }

        public string Value { get; set; }
    }

    public class PinInfo
    {
        public Pixel Anchor { get; set; }

        public Pixel BottomRightOffset { get; set; }

        public Pixel TopLeftOffset { get; set; }

        public Point Point { get; set; }
    }

    public class Pixel
    {
        public string X { get; set; }

        public string Y { get; set; }
    }

    public class Point : Shape
    {
        public string Type { get; set; }

        public double[] Coordinates { get; set; }

        public string CalculationMethod { get; set; }

        public string[] UsageTypes { get; set; }
    }

    public class Resource
    {
        public double[] BoundingBox { get; set; }

        public string Type { get; set; }
    }

    public class ResourceSet
    {
        public long EstimatedTotal { get; set; }

        public Resource[] Resources { get; set; }
    }

    public class Response
    {
        public string Copyright { get; set; }

        public string BrandLogoUri { get; set; }

        public int StatusCode { get; set; }

        public string StatusDescription { get; set; }

        public string AuthenticationResultCode { get; set; }

        public string[] errorDetails { get; set; }

        public string TraceId { get; set; }

        public ResourceSet[] ResourceSets { get; set; }
    }

    public class RoadShield
    {
        public int Bucket { get; set; }

        public Shield[] Shields { get; set; }
    }

    public class Route : Resource
    {
        public string Id { get; set; }

        public string DistanceUnit { get; set; }

        public string DurationUnit { get; set; }

        public double TravelDistance { get; set; }

        public double TravelDuration { get; set; }

        public RouteLeg[] RouteLegs { get; set; }

        public RoutePath RoutePath { get; set; }
    }

    public class RouteLeg
    {
        public double TravelDistance { get; set; }

        public double TravelDuration { get; set; }

        public Point ActualStart { get; set; }

        public Point ActualEnd { get; set; }

        public Location StartLocation { get; set; }

        public Location EndLocation { get; set; }

        public ItineraryItem[] ItineraryItems { get; set; }
    }

    public class RoutePath
    {
        public Line Line { get; set; }

        public Generalization[] Generalizations { get; set; }
    }

    public class Shape
    {
        public double[] BoundingBox { get; set; }
    }

    public class Shield
    {
        public string[] Labels { get; set; }

        public int RoadShieldType { get; set; }
    }

    public class StaticMapMetadata : ImageryMetadata
    {
        public Point MapCenter { get; set; }

        public PinInfo[] Pushpins { get; set; }

        public string Zoom { get; set; }
    }

    public class TrafficIncident : Resource
    {
        public Point Point { get; set; }

        public string Congestion { get; set; }

        public string Description { get; set; }

        public string Detour { get; set; }

        public string Start { get; set; }

        public string End { get; set; }

        public long IncidentId { get; set; }

        public string Lane { get; set; }

        public string LastModified { get; set; }

        public bool RoadClosed { get; set; }

        public int Severity { get; set; }

        public Point ToPoint { get; set; }

        public string[] LocationCodes { get; set; }

        public int Type { get; set; }

        public bool Verified { get; set; }
    }

    public class TransitLine
    {
        public string verboseName { get; set; }

        public string abbreviatedName { get; set; }

        public long AgencyId { get; set; }

        public string agencyName { get; set; }

        public long lineColor { get; set; }

        public long lineTextColor { get; set; }

        public string uri { get; set; }

        public string phoneNumber { get; set; }

        public string providerInfo { get; set; }
    }

    public class Warning
    {
        public string WarningType { get; set; }

        public string Severity { get; set; }

        public string Text { get; set; }
    }

    public class CompressedPointList : Resource
    {
        public string Value { get; set; }
    }

    public class ElevationData : Resource
    {
        public int[] Elevations { get; set; }

        public int ZoomLevel { get; set; }
    }

    public class SeaLevelData : Resource
    {
        public int[] Offsets { get; set; }

        public int ZoomLevel { get; set; }
    }

    internal class BingDataConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return typeof(Resource).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jObject = JObject.Load(reader);
            var type = jObject["__type"].Value<string>().Split(':')[0];

            object target = null;

            switch (type)
            {
                case "Location":
                    target = new Location();
                    break;
                case "Route":
                    target = new Route();
                    break;
                case "TrafficIncident":
                    target = new TrafficIncident();
                    break;
                case "ImageryMetadata":
                    target = new ImageryMetadata();
                    break;
                case "ElevationData":
                    target = new ElevationData();
                    break;
                case "SeaLevelData":
                    target = new SeaLevelData();
                    break;
                case "CompressedPointList":
                    target = new CompressedPointList();
                    break;
                default:
                    throw new ArgumentException("Invalid source type");
            }

            serializer.Populate(jObject.CreateReader(), target);

            return target;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
