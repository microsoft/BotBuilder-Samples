using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Bot.Models
{
    [Serializable]
    public sealed class Note
    {
        [BsonId]
        public string DocumentId { get; set; }

        [BsonElement]
        public string NoteOwnerId { get; set; }

        [BsonElement]
        public string NoteContent { get; set; }

        [BsonElement]
        public long TimestampTicks { get; set; }

        [BsonElement]
        public List<string> HashTagsUsed { get; set; }
    }
}