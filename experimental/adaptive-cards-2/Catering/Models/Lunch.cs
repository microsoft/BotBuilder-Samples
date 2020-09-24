using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catering.Models
{
    public class Lunch
    {
        [JsonProperty("orderTimestamp")]
        public DateTime OrderTimestamp { get; set; }

        [JsonProperty("drink")]
        public string Drink { get; set; }

        [JsonProperty("entre")]
        public string Entre { get; set; }
    }
}
