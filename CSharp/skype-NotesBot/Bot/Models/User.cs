using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Bot.Models
{
    [Serializable]
    public sealed class User
    {
        [BsonId]
        public string DocumentId { get; set; }

        [BsonElement]
        public string UserId { get; set; }

        [BsonElement]
        public string UserName { get; set; }

        [BsonElement]
        public string BotId { get; set; }

        [BsonElement]
        public string BotName { get; set; }

        [BsonElement]
        public string ServiceUrl { get; set; }

        [BsonElement]
        public string ChannelId { get; set; }

        [BsonElement]
        public string ConversationId { get; set; }

        [BsonElement]
        public List<string> GroupIds { get; set; }

        [BsonElement]
        public bool IsFriend { get; set; }

        [BsonElement]
        public List<TaggedNote> TaggedNotes { get; set; }

        [BsonElement]
        public List<Note> SavedNotes { get; set; }
    }
}