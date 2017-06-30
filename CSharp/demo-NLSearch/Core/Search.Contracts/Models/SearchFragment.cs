using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Search.Models
{
#if !NETSTANDARD1_6
    [Serializable]
#else
    [JsonObject(MemberSerialization.OptOut)]
#endif
    public class SearchFragment
    {
        public Synonyms Phrases { get; set; }
        public SearchSpec Fragment { get; set; }
    }
}
