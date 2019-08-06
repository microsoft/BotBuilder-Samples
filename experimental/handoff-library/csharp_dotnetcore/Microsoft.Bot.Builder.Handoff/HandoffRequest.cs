// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

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
    public class HandoffRequest : IHandoffRequest
    {
        private readonly string conversationId;
        private readonly IConversations conversations;

        public HandoffRequest(string conversationId, IConversations conversations)
        {
            this.conversationId = conversationId;
            this.conversations = conversations;
        }

        public async Task<bool> IsCompletedAsync()
        {
            try
            {
                var result = await HandoffHttpSupport.GetHandoffStatusWithHttpMessagesAsync((IServiceOperations<ConnectorClient>)conversations, conversationId).ConfigureAwait(false);
                return result.Body.ToLower() == "completed";
            }
            catch
            {
                // This included not found
            }

            return false;
        }
    }
}
