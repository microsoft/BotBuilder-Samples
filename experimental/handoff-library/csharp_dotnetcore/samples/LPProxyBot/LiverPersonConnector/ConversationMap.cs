// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Concurrent;
using Microsoft.Bot.Schema;

namespace LivePersonConnector
{
    public class ConversationRecord
    {
        public ConversationReference ConversationReference;
        public bool IsAcked = false;
        public bool IsClosed = false;
    }

    public class ConversationMap
    {
        public ConcurrentDictionary<string, ConversationRecord> ConversationRecords = new ConcurrentDictionary<string, ConversationRecord>();
    }
}
