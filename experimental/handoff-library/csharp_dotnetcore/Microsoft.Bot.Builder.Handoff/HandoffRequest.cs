// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Handoff;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Microsoft.Rest;

namespace Microsoft.Bot.Builder
{
    /// <summary>
    /// The interface allows callers to track the completion of the handoff request.
    /// </summary>
    public class HandoffRequest
    {
        private readonly string _conversationId;
        private readonly IConversations _conversations;

        internal HandoffRequest(string conversationId, IConversations conversations)
        {
            this._conversationId = conversationId ?? throw new ArgumentNullException(nameof(conversationId));
            this._conversations = conversations ?? throw new ArgumentNullException(nameof(conversations));
        }

        public virtual async Task<bool> IsCompletedAsync()
        {
            var result = await HandoffHttpSupport.GetHandoffStatusWithHttpMessagesAsync((IServiceOperations<ConnectorClient>)_conversations, _conversationId).ConfigureAwait(false);
            return result.Body.ToLower(CultureInfo.InvariantCulture) == "completed";
        }
    }
}
