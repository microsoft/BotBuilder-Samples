// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using Microsoft.Bot.Schema;

namespace LivePersonConnector
{
    public class ConversationRecord
    {
        public ConversationReference ConversationReference { get; set; }
        public bool IsAcknowledged { get; set; } = false;
        public bool IsClosed { get; set; } = false;
    }

    public class ConversationMap
    {
        public ConcurrentDictionary<string, ConversationRecord> ConversationRecords { get; set; } = new ConcurrentDictionary<string, ConversationRecord>();
    }
}
