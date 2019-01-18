using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotFileCreator.BotFileWriter
{
    public class BotFile
    {
        public string name { get; set; }
        public string description { get; set; }
        public string padlock { get; set; }
        public string version { get; set; }

        public List<BotService> services { get; set; }

        public BotFile()
        {
        }
    }
}
