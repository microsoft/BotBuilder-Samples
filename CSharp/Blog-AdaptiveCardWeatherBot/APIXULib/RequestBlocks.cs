using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIXULib
{
    public static class MethodTypeParemeters
    {
        public static string Current { get { return "/current.json"; } }

        public static string Forecast { get { return "/forecast.json"; } }

        public static string GetParameters(this MethodType methodType)
        {
            string methodPara = "";
            switch (methodType)
            {
                case MethodType.Current:
                    methodPara = MethodTypeParemeters.Current;
                    break;
                case MethodType.Forecast:
                    methodPara = MethodTypeParemeters.Forecast;
                    break;

            }
            if (string.IsNullOrEmpty(methodPara))
            {
                throw new Exception("Invalid Method Type");
            }
            else
            {
                return methodPara;

            }
        }


    }

    public enum MethodType
    {
        Current,
        Forecast
    }

    public enum Days
    {
        One = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10
    }

    public enum GetBy
    {
        CityName,
        Zip,
        PostCode,
        PostalCode,
        Metar,
        iata,
        IPAddress
    }

    public static class ReqestFor
    {
        public static string City(string cityName)
        {
            return "q=" + cityName;
        }

        public static string Zip(string zip)
        {
            return "q=" + zip;
        }

        public static string PostCode(string postcode)
        {
            return "q=" + postcode;
        }

        public static string PostalCode(string postalCode)
        {
            return "q=" + postalCode;
        }

        public static string LatLong(string latitude, string longitude)
        {
            return "q=" + latitude + "," + longitude;
        }

        public static string Metar(string metar)
        {
            return "q=metar:" + metar;
        }

        public static string iata(string iata)
        {
            return "q=iata:" + iata;
        }

        public static string AutoIP()
        {
            return "q=auto:ip";
        }

        public static string IPAddress(string IP)
        {
            return "q=" + IP;
        }

        public static string PrepareQueryParameter(this GetBy getby, string value)
        {
            string queryParameter = "";
            switch (getby)
            {
                case GetBy.CityName:
                    queryParameter = ReqestFor.City(value);
                    break;
                case GetBy.Zip:
                    queryParameter = ReqestFor.Zip(value);
                    break;
                case GetBy.PostCode:
                    queryParameter = ReqestFor.PostCode(value);
                    break;
                case GetBy.PostalCode:
                    queryParameter = ReqestFor.PostalCode(value);
                    break;
                case GetBy.Metar:
                    queryParameter = ReqestFor.Metar(value);
                    break;
                case GetBy.iata:
                    queryParameter = ReqestFor.iata(value);
                    break;
                case GetBy.IPAddress:
                    queryParameter = ReqestFor.IPAddress(value);
                    break;
            }
            return queryParameter;
        }
        public static string PrepareDays(this Days days)
        {
            return "days=" + (int) days;
        }


    }
}
