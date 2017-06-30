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
    public class Synonyms
    {
        public string Canonical { get; set; }

        public string[] Alternatives { get; set; }

        public string Description
        {
            get { return Alternatives.FirstOrDefault(); }
        }

        public Synonyms(string canonical, params string[] alternatives)
        {
            Canonical = canonical;
            Alternatives = alternatives;
        }
    }
}
