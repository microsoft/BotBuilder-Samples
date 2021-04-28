// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Azure.Cosmos.Table;
using storage = global::Microsoft.Azure.Cosmos.Table;

namespace Bot.Builder.Azure.V3V4.AzureTable
{
    /// <summary>
    /// <see cref="IBotDataStore{T}"/> Implementation using Azure Storage Table
    /// From https://github.com/microsoft/BotBuilder-Azure
    /// </summary>
    /// <notes>
    /// This implementation stores all conversation data in one partition which will not scale for heavy use.  If you think
    /// you will have high traffic load you should use TableBotDataStore2 which splits data between multiple tables with fine grained partitionkeys.
    /// If you have need for geo-distributed datacenters and or deeper queries (like for supporting GDPR requirements) then you are strongly encouraged 
    /// to use the DocumentDbBotDataStore (aka CosmosDB) 
    /// </notes>
    public class TableBotDataStore : IBotDataStore<BotData>
    {
        private static HashSet<string> checkedTables = new HashSet<string>();

        /// <summary>
        /// Creates an instance of the <see cref="IBotDataStore{T}"/> that uses the azure table storage.
        /// </summary>
        /// <param name="connectionString">The storage connection string.</param>
        /// <param name="tableName">The name of table.</param>
        public TableBotDataStore(string connectionString, string tableName = "botdata")
            : this(storage.CloudStorageAccount.Parse(connectionString), tableName)
        {
        }

        /// <summary>
        /// Creates an instance of the <see cref="IBotDataStore{T}"/> that uses the azure table storage.
        /// </summary>
        /// <param name="storageAccount">The storage account.</param>
        /// <param name="tableName">The name of table.</param>
        public TableBotDataStore(storage.CloudStorageAccount storageAccount, string tableName = "botdata")
        {
            var tableClient = storageAccount.CreateCloudTableClient();
            this.Table = tableClient.GetTableReference(tableName);

            lock (checkedTables)
            {
                if (!checkedTables.Contains(tableName))
                {
                    this.Table.CreateIfNotExists();
                    checkedTables.Add(tableName);
                }
            }
        }

        /// <summary>
        /// Creates an instance of the <see cref="IBotDataStore{T}"/> that uses the azure table storage.
        /// </summary>
        /// <param name="table">The cloud table.</param>
        public TableBotDataStore(CloudTable table)
        {
            this.Table = table;
        }

        /// <summary>
        /// The <see cref="CloudTable"/>.
        /// </summary>
        public CloudTable Table { get; private set; }

        async Task<BotData> IBotDataStore<BotData>.LoadAsync(IAddress key, BotStoreType botStoreType, CancellationToken cancellationToken)
        {
            var entityKey = BotDataEntity.GetEntityKey(key, botStoreType);
            var result = await this.Table.ExecuteAsync(TableOperation.Retrieve<BotDataEntity>(entityKey.PartitionKey, entityKey.RowKey));
            BotDataEntity entity = (BotDataEntity)result.Result;
            if (entity == null)
                // empty record ready to be saved
                return new BotData(eTag: String.Empty, data: null);

            // return botdata 
            return new BotData(entity.ETag, entity.GetData());

        }

        async Task IBotDataStore<BotData>.SaveAsync(IAddress key, BotStoreType botStoreType, BotData botData, CancellationToken cancellationToken)
        {
            var entityKey = BotDataEntity.GetEntityKey(key, botStoreType);
            BotDataEntity entity = new BotDataEntity(key.BotId, key.ChannelId, key.ConversationId, key.UserId, botData.Data)
            {
                ETag = botData.ETag
            };
            entity.PartitionKey = entityKey.PartitionKey;
            entity.RowKey = entityKey.RowKey;

            if (String.IsNullOrEmpty(entity.ETag))
                await this.Table.ExecuteAsync(TableOperation.Insert(entity));
            else if (entity.ETag == "*")
            {
                if (botData.Data != null)
                    await this.Table.ExecuteAsync(TableOperation.InsertOrReplace(entity));
                else
                    await this.Table.ExecuteAsync(TableOperation.Delete(entity));
            }
            else
            {
                if (botData.Data != null)
                    await this.Table.ExecuteAsync(TableOperation.Replace(entity));
                else
                    await this.Table.ExecuteAsync(TableOperation.Delete(entity));
            }
        }

        Task<bool> IBotDataStore<BotData>.FlushAsync(IAddress key, CancellationToken cancellationToken)
        {
            // Everything is saved. Flush is no-op
            return Task.FromResult(true);
        }
    }
}
