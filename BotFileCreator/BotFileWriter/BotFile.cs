// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace BotFileCreator.BotFileWriter
{
    using System.Collections.Generic;
 
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
