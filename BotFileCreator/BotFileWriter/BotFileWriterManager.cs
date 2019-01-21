// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace BotFileCreator.BotFileWriter
{
    using Newtonsoft.Json;
    using System.IO;

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
