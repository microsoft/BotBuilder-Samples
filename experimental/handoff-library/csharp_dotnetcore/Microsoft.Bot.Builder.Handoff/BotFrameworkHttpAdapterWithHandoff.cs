// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Handoff;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Rest;

namespace Microsoft.Bot.Builder.Handoff
{
    /// <summary>
    /// HTTP adapter that supports handoff
    /// When handoff is included in the SDK proper, this class becomes unnecessary.
    /// </summary>
    public class BotFrameworkHttpAdapterWithHandoff : BotFrameworkHttpAdapter, IHandoffAdapter
    {
        public BotFrameworkHttpAdapterWithHandoff(IConfiguration configuration, ILogger<BotFrameworkHttpAdapter> logger = null)
            : base(configuration, logger)
        {
        }

        internal BotFrameworkHttpAdapterWithHandoff()
        {
        }

        async Task<HandoffRequest> IHandoffAdapter.InitiateHandoffAsync(ITurnContext turnContext, Activity[] activities, object handoffContext, CancellationToken cancellationToken)
        {
            var connectorClient = turnContext.TurnState.Get<IConnectorClient>();

            string conversationId = turnContext.Activity.Conversation.Id;
            var handoffParameters = new HandoffParameters(new Transcript { Activities = activities }, handoffContext);
            var response = await HandoffHttpSupport.HandoffWithHttpMessagesAsync((IServiceOperations<ConnectorClient>)connectorClient.Conversations, conversationId, handoffParameters, cancellationToken).ConfigureAwait(false);

            return new HandoffRequest(conversationId, connectorClient.Conversations);
        }
    }
}
