using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotFileCreator.BotFileWriter
{
    public static class BotFileWriterManager
    {
        public static void WriteBotFile(BotFile botFile, string botFileFullPath)
        {
            using (StreamWriter file = File.CreateText(botFileFullPath))
            {
                JsonSerializer serializer = new JsonSerializer();

                serializer.Serialize(file, botFile);
            }
        }
    }
}
