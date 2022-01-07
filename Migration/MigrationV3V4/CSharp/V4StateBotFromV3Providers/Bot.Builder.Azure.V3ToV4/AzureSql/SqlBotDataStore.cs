// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Bot.Builder.Azure.V3V4.AzureSql
{
    /// <summary>
    /// <see cref="IBotDataStore{BotData}"/> Implementation using Azure SQL 
    /// From https://github.com/microsoft/BotBuilder-Azure
    /// </summary>
    public class SqlBotDataStore : IBotDataStore<BotData>
    {
        string _connectionString { get; set; }

        /// <summary>
        /// Creates an instance of the <see cref="IBotDataStore{BotData}"/> that uses the azure sql storage.
        /// </summary>
        /// <param name="connectionString">The storage connection string.</param>
        public SqlBotDataStore(string connectionString)
        {
            _connectionString = connectionString;
        }

        async Task<BotData> IBotDataStore<BotData>.LoadAsync(IAddress key, BotStoreType botStoreType, CancellationToken cancellationToken)
        {
            using (var context = new SqlBotDataContext(_connectionString))
            {
                    SqlBotDataEntity entity = await SqlBotDataEntity.GetSqlBotDataEntity(key, botStoreType, context);

                    if (entity == null)
                        // empty record ready to be saved
                        return new BotData(eTag: String.Empty, data: new Dictionary<string, object>());

                    // return botdata 
                    return new BotData(entity.ETag, entity.GetData());
            }
        }

        async Task IBotDataStore<BotData>.SaveAsync(IAddress key, BotStoreType botStoreType, BotData botData, CancellationToken cancellationToken)
        {
            SqlBotDataEntity entity = new SqlBotDataEntity(botStoreType, key.BotId, key.ChannelId, key.ConversationId, key.UserId, botData.Data)
            {
                ETag = botData.ETag,
                ServiceUrl = key.ServiceUrl
            };

            using (var context = new SqlBotDataContext(_connectionString))
            {
                    if (string.IsNullOrEmpty(botData.ETag))
                    {
                        entity.ETag = Guid.NewGuid().ToString();
                        context.BotData.Add(entity);
                    }
                    else if (entity.ETag == "*")
                    {
                        var foundData = await SqlBotDataEntity.GetSqlBotDataEntity(key, botStoreType, context);
                        if (botData.Data != null)
                        {
                            if (foundData == null)
                                context.BotData.Add(entity);
                            else
                            {
                                foundData.Data = entity.Data;
                                foundData.ServiceUrl = entity.ServiceUrl;
                            }
                        }
                        else
                        {
                            if (foundData != null)
                                context.BotData.Remove(foundData);
                        }
                    }
                    else
                    {
                        var foundData = await SqlBotDataEntity.GetSqlBotDataEntity(key, botStoreType, context);
                        if (botData.Data != null)
                        {
                            if (foundData == null)
                                context.BotData.Add(entity);
                            else
                            {
                                foundData.Data = entity.Data;
                                foundData.ServiceUrl = entity.ServiceUrl;
                                foundData.ETag = entity.ETag;
                            }
                        }
                        else
                        {
                            if (foundData != null)
                                context.BotData.Remove(foundData);
                        }
                    }
                    await context.SaveChangesAsync();
            }
        }

        Task<bool> IBotDataStore<BotData>.FlushAsync(IAddress key, CancellationToken cancellationToken)
        {
            // Everything is saved. Flush is no-op
            return Task.FromResult(true);
        }
    }
}