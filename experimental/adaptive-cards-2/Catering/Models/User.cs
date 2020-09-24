using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Catering.Models
{
    public class User
    {
        public User()
        {
            this.Lunch = new Lunch();
        }

        [JsonProperty("id")]
        public string Id { get; set; }
        
        [JsonProperty("partitionKey")]
        public string PartitionKey { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
        
        [JsonProperty("lunch")]
        public Lunch Lunch { get; set; }
    }
}
