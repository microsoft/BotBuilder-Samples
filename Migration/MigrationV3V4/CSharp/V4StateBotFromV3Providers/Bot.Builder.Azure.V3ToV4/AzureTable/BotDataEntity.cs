// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.
using System;
using Newtonsoft.Json;
using System.IO;
using System.IO.Compression;
using Newtonsoft.Json.Linq;
using Microsoft.Azure.Cosmos.Table;

namespace Bot.Builder.Azure.V3V4.AzureTable
{
    /// <summary>
    /// This is the entity stored by <see cref="TableBotDataStore2"/>. 
    /// From https://github.com/microsoft/BotBuilder-Azure
    /// </summary>
    internal class BotDataEntity : TableEntity
    {
        private static readonly JsonSerializerSettings serializationSettings = new JsonSerializerSettings()
        {
            Formatting = Formatting.None,
            NullValueHandling = NullValueHandling.Ignore
        };

        public BotDataEntity()
        {
        }

        internal BotDataEntity(string botId, string channelId, string conversationId, string userId, object data)
        {
            this.BotId = botId;
            this.ChannelId = channelId;
            this.ConversationId = conversationId;
            this.UserId = userId;
            this.Data = Serialize(data);
        }

        private byte[] Serialize(object data)
        {
            using (var cmpStream = new MemoryStream())
            using (var stream = new GZipStream(cmpStream, CompressionMode.Compress))
            using (var streamWriter = new StreamWriter(stream))
            {
                var serializedJSon = JsonConvert.SerializeObject(data, serializationSettings);
                streamWriter.Write(serializedJSon);
                streamWriter.Close();
                stream.Close();
                return cmpStream.ToArray();
            }
        }

        private object Deserialize(byte[] bytes)
        {
            using (var stream = new MemoryStream(bytes))
            using (var gz = new GZipStream(stream, CompressionMode.Decompress))
            using (var streamReader = new StreamReader(gz))
            {
                return JsonConvert.DeserializeObject(streamReader.ReadToEnd());
            }
        }


        internal static EntityKey GetEntityKey(IAddress key, BotStoreType botStoreType)
        {
            switch (botStoreType)
            {
                case BotStoreType.BotConversationData:
                    return new EntityKey($"{key.ChannelId}:conversation", key.ConversationId.SanitizeForAzureKeys());

                case BotStoreType.BotUserData:
                    return new EntityKey($"{key.ChannelId}:user", key.UserId.SanitizeForAzureKeys());

                case BotStoreType.BotPrivateConversationData:
                    return new EntityKey($"{key.ChannelId}:private", $"{key.ConversationId.SanitizeForAzureKeys()}:{key.UserId.SanitizeForAzureKeys()}");

                default:
                    throw new ArgumentException("Unsupported bot store type!");
            }
        }

        internal ObjectT GetData<ObjectT>()
        {
            return ((JObject)Deserialize(this.Data)).ToObject<ObjectT>();
        }

        internal object GetData()
        {
            return Deserialize(this.Data);
        }

        public string BotId { get; set; }

        public string ChannelId { get; set; }

        public string ConversationId { get; set; }

        public string UserId { get; set; }

        public byte[] Data { get; set; }
    }

}
