// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using System.Collections.Generic;

namespace LivePersonConnector
{
    internal class EscalationsConversationData
    {
        public LivePersonConversationRecord EscalationRecord { get; set; } = null;
    }
}