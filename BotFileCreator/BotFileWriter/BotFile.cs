// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace BotFileCreator.BotFileWriter
{
    using System.Collections.Generic;

    public class BotFile
    {
        public BotFile()
        {
        }

        public string Name { get; set; }

        public string Description { get; set; }

        public string Padlock { get; set; }

        public string Version { get; set; }

        public List<BotService> Services { get; set; }
    }
}
