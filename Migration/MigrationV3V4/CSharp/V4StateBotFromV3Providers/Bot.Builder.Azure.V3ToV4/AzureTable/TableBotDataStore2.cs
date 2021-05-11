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
    /// <see cref="IBotDataStore{T}"/> Implementation using Azure Table Storage,
    /// From https://github.com/microsoft/BotBuilder-Azure
    /// </summary>
    /// <notes>
    /// The original TableBotDataStore put all conversation data in 1 partition in 1 table which severely limited the scalability of it. 
    /// The TableBotDataStore2 implementaion uses multiple tables, and the UserId or ConversationId as the PartitionKey within those tables to 
    /// achieve much higher scalability charateristics.
    /// 
    /// tableName="{BotId}{ChannelId}"
    /// BotStoreType             | PartitionKey      | RowKey         |
    /// ---------------------------------------------------------------
    /// UserData                 | {UserId}          | "user"         |
    /// ConverationData          | {ConversationId}  | "conversation" |
    /// PrivateConverationData   | {ConversationId}  | {UserId}       |
    /// ---------------------------------------------------------------
    /// </notes>
    public class TableBotDataStore2 : IBotDataStore<BotData>
    {
        private static Dictionary<string, CloudTable> tables = new Dictionary<string, CloudTable>();
        private CloudTableClient tableClient;

        /// <summary>
        /// Creates an instance of the <see cref="IBotDataStore{T}"/> that uses the azure table storage.
        /// </summary>
        /// <param name="connectionString">The storage connection string.</param>
        /// <param name="tableName">The name of table.</param>
        public TableBotDataStore2(string connectionString)
            : this(storage.CloudStorageAccount.Parse(connectionString))
        {
        }

        /// <summary>
        /// Creates an instance of the <see cref="IBotDataStore{T}"/> that uses the azure table storage.
        /// </summary>
        /// <param name="storageAccount">The storage account.</param>
        /// <param name="tableName">The name of table.</param>
        public TableBotDataStore2(storage.CloudStorageAccount storageAccount)
        {
            tableClient = storageAccount.CreateCloudTableClient();
        }

        public IEnumerable<CloudTable> Tables { get { return tables.Values; } }

        private CloudTable GetTable(IAddress key)
        {
            string tableName = $"bd{key.BotId}{key.ChannelId}".SanitizeTableName();
            lock (tables)
            {
                CloudTable table;
                if (tables.TryGetValue(tableName, out table))
                    return table;

                table = tableClient.GetTableReference(tableName);
                table.CreateIfNotExists();
                tables.Add(tableName, table);
                return table;
            }
        }

        internal static EntityKey GetEntityKey(IAddress key, BotStoreType botStoreType)
        {
            switch (botStoreType)
            {
                case BotStoreType.BotUserData:
                    return new EntityKey(key.UserId, "user");

                case BotStoreType.BotConversationData:
                    return new EntityKey(key.ConversationId, "conversation");

                case BotStoreType.BotPrivateConversationData:
                    return new EntityKey(key.ConversationId, key.UserId);

                default:
                    throw new ArgumentException("Unsupported bot store type!");
            }
        }


        async Task<BotData> IBotDataStore<BotData>.LoadAsync(IAddress address, BotStoreType botStoreType, CancellationToken cancellationToken)
        {
            var table = this.GetTable(address);
            var entityKey = GetEntityKey(address, botStoreType);

            var result = await table.ExecuteAsync(TableOperation.Retrieve<BotDataEntity>(entityKey.PartitionKey, entityKey.RowKey));
            BotDataEntity entity = (BotDataEntity)result.Result;
            if (entity == null)
                // empty record ready to be saved
                return new BotData(eTag: String.Empty, data: null);

            // return botdata 
            return new BotData(entity.ETag, entity.GetData());
        }

        async Task IBotDataStore<BotData>.SaveAsync(IAddress address, BotStoreType botStoreType, BotData botData, CancellationToken cancellationToken)
        {
            var table = this.GetTable(address);
            var entityKey = GetEntityKey(address, botStoreType);
            BotDataEntity entity = new BotDataEntity(address.BotId, address.ChannelId, address.ConversationId, address.UserId, botData.Data)
            {
                ETag = botData.ETag
            };
            entity.PartitionKey = entityKey.PartitionKey;
            entity.RowKey = entityKey.RowKey;

            if (String.IsNullOrEmpty(entity.ETag))
                await table.ExecuteAsync(TableOperation.Insert(entity));
            else if (entity.ETag == "*")
            {
                if (botData.Data != null)
                    await table.ExecuteAsync(TableOperation.InsertOrReplace(entity));
                else
                    await table.ExecuteAsync(TableOperation.Delete(entity));
            }
            else
            {
                if (botData.Data != null)
                    await table.ExecuteAsync(TableOperation.Replace(entity));
                else
                    await table.ExecuteAsync(TableOperation.Delete(entity));
            }
        }

        Task<bool> IBotDataStore<BotData>.FlushAsync(IAddress key, CancellationToken cancellationToken)
        {
            // Everything is saved. Flush is no-op
            return Task.FromResult(true);
        }
    }
}
