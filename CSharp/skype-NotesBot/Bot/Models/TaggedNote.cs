using System;
using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace Bot.Models
{
    [Serializable]
    public sealed class TaggedNote
    {
        [BsonElement]
        public string TagValue { get; set; }

        [BsonElement]
        public List<string> NoteIds { get; set; }
    }
}