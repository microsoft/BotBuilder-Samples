using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APIXULib
{
    public static class RequestBuilder
    {
        public  static string PrepareRequest( MethodType methodType,string key, GetBy getBy, string value, Days ofDays)
        {
            return string.Concat(methodType.GetParameters(),"?key=",key,"&", getBy.PrepareQueryParameter(value),"&", ofDays.PrepareDays());
        }

        public static string PrepareRequest(MethodType methodType, string key, GetBy getBy, string value)
        {
            return string.Concat(methodType.GetParameters(), "?key=", key, "&", getBy.PrepareQueryParameter(value));
        }

        public static string PrepareRequestByLatLong(MethodType methodType, string key, string latitude, string longitude, Days ofDays)
        {
            return string.Concat(methodType.GetParameters(), "?key=", key, "&", ReqestFor.LatLong(latitude,longitude) , "&", ofDays.PrepareDays());
        }

        public static string PrepareRequestByLatLong(MethodType methodType, string key, string latitude, string longitude)
        {
            return string.Concat(methodType.GetParameters(), "?key=", key, "&", ReqestFor.LatLong(latitude, longitude));
        }

        public static string PrepareRequestByAutoIP(MethodType methodType, string key, Days ofDays )
        {
            return string.Concat(methodType.GetParameters(), "?key=", key, "&", ReqestFor.AutoIP(), "&", ofDays.PrepareDays());
        }

        public static string PrepareRequestByAutoIP(MethodType methodType, string key)
        {
            return string.Concat(methodType.GetParameters(), "?key=", key, "&", ReqestFor.AutoIP());
        }
    }
}
