using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TeamsMessagingExtensionsSearch
{
    
    public class ConnectorJsonSerializer
    {
        public string context { get; set; }
        public string type { get; set; }
        public Section[] sections { get; set; }
        public Potentialaction[] potentialAction { get; set; }
    }

    public class Section
    {
        public string activityTitle { get; set; }
        public string activitySubtitle { get; set; }
        public string activityImage { get; set; }
        public Fact[] facts { get; set; }
        public string text { get; set; }
    }

    public class Fact
    {
        public string name { get; set; }
        public string value { get; set; }
    }

    public class Potentialaction
    {
        public string context { get; set; }
        public string type { get; set; }
        public string name { get; set; }
        public string[] target { get; set; }
    }

}
