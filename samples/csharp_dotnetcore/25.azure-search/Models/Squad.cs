using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Botv4AzureSearch.Models
{
    public class Squad
    {
        public float searchscore { get; set; }

        public string paragraph_index { get; set; }

        public string paragraph_text { get; set; }

        public string keyphrases { get; set; }

        // public string[] keyphrases { get; set; }
    }
}
