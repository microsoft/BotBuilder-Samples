// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
using System;
using Newtonsoft.Json;

namespace Bot.Builder.Azure.V3V4.CosmosDb
{
    /// <summary>
    /// This is the key of the record stored by <see cref="DocumentDbBotDataStore"/>. 
    /// From https://github.com/microsoft/BotBuilder-Azure
    /// </summary>
    internal class DocDbBotDataEntity
    {
        internal const int MAX_KEY_LENGTH = 254;
        public DocDbBotDataEntity() { }

        internal DocDbBotDataEntity(IAddress key, BotStoreType botStoreType, BotData botData)
        {
            this.Id = GetEntityKey(key, botStoreType);
            this.BotId = key.BotId;
            this.ChannelId = key.ChannelId;
            this.ConversationId = key.ConversationId;
            this.UserId = key.UserId;
            this.Data = botData.Data;
        }

        public static string GetEntityKey(IAddress key, BotStoreType botStoreType)
        {
            string entityKey;
            switch (botStoreType)
            {
                case BotStoreType.BotConversationData:
                    entityKey = $"{key.ChannelId}:conversation{key.ConversationId.SanitizeForAzureKeys()}";
                    return TruncateEntityKey(entityKey);

                case BotStoreType.BotUserData:
                    entityKey = $"{key.ChannelId}:user{key.UserId.SanitizeForAzureKeys()}";
                    return TruncateEntityKey(entityKey);

                case BotStoreType.BotPrivateConversationData:
                    entityKey = $"{key.ChannelId}:private{key.ConversationId.SanitizeForAzureKeys()}:{key.UserId.SanitizeForAzureKeys()}";
                    return TruncateEntityKey(entityKey);

                default:
                    throw new ArgumentException("Unsupported bot store type!");
            }
        }

        private static string TruncateEntityKey(string entityKey)
        {
            if (entityKey.Length > MAX_KEY_LENGTH)
            {
                var hash = entityKey.GetHashCode().ToString("x");
                entityKey = entityKey.Substring(0, MAX_KEY_LENGTH - hash.Length) + hash;
            }

            return entityKey;
        }

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "botId")]
        public string BotId { get; set; }

        [JsonProperty(PropertyName = "channelId")]
        public string ChannelId { get; set; }

        [JsonProperty(PropertyName = "conversationId")]
        public string ConversationId { get; set; }

        [JsonProperty(PropertyName = "userId")]
        public string UserId { get; set; }

        [JsonProperty(PropertyName = "data")]
        public object Data { get; set; }
    }
}



