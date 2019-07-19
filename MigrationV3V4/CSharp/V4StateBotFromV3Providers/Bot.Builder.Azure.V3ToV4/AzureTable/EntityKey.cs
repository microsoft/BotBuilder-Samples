// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
namespace Bot.Builder.Azure.V3V4.AzureTable
{
    /// <summary>
    /// This is the key of the record stored by <see cref="TableBotDataStore2"/>. 
    /// From https://github.com/microsoft/BotBuilder-Azure
    /// </summary>
    internal class EntityKey
    {
        public EntityKey(string partition, string row)
        {
            PartitionKey = partition.SanitizeForAzureKeys();
            RowKey = row.SanitizeForAzureKeys();
        }

        public string PartitionKey { get; private set; }
        public string RowKey { get; private set; }

    }

}
