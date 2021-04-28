// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
namespace Bot.Builder.Azure.V3V4.CosmosDb
{    
    /// <summary>
    /// This is the key of the record stored by <see cref="DocumentDbBotDataStore"/>. 
    /// From https://github.com/microsoft/BotBuilder-Azure
    /// </summary>
    internal class BotDataDocDbKey
    {
        public BotDataDocDbKey(string partition, string row)
        {
            PartitionKey = partition;
            RowKey = row;
        }

        public string PartitionKey { get; private set; }
        public string RowKey { get; private set; }

    }
}



