// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace LivePersonConnector
{
    public class HandoffMiddleware : IMiddleware
    {
        private readonly BotState _conversationState;
        private readonly ConversationMap _conversationMap;
        private readonly ILivePersonCredentialsProvider _creds;

        public HandoffMiddleware(ConversationState conversationState, ConversationMap conversationMap, ILivePersonCredentialsProvider creds)
        {
            _conversationState = conversationState;
            _conversationMap = conversationMap;
            _creds = creds;
        }

        public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default)
        {
            // Route the conversation based on whether it's been escalated
            var conversationStateAccessors = _conversationState.CreateProperty<EscalationsConversationData>(nameof(EscalationsConversationData));
            var conversationData = await conversationStateAccessors.GetAsync(turnContext, () => new EscalationsConversationData()).ConfigureAwait(false);

            if (turnContext.Activity.Type == ActivityTypes.Message && conversationData.EscalationRecord != null)
            {
                var account = _creds.LpAccount;
                var message = LivePersonConnector.MakeLivePersonMessage(0, conversationData.EscalationRecord.ConversationId, turnContext.Activity.Text);

                await LivePersonConnector.SendMessageToConversationAsync(account,
                    conversationData.EscalationRecord.MessageDomain,
                    conversationData.EscalationRecord.AppJWT,
                    conversationData.EscalationRecord.ConsumerJWS,
                    message).ConfigureAwait(false);
                return;
            }

            if (turnContext.Activity.Type == ActivityTypes.Event && turnContext.Activity.Name == HandoffEventNames.HandoffStatus)
            {
                try
                {
                    var state = (turnContext.Activity.Value as JObject)?.Value<string>("state");
                    if (state == "completed")
                    {
                        conversationData.EscalationRecord = null;
                        await _conversationState.SaveChangesAsync(turnContext).ConfigureAwait(false);
                    }
                }
                catch { }
            }

            turnContext.OnSendActivities(async (sendTurnContext, activities, nextSend) =>
            {
                // Handle any escalation events, and let them propagate through the pipeline
                // This is useful for debugging with the Emulator
                var handoffEvents = activities.Where(activity =>
                    activity.Type == ActivityTypes.Event && activity.Name == HandoffEventNames.InitiateHandoff);

                if (handoffEvents.Count() == 1)
                {
                    var handoffEvent = handoffEvents.First();
                    conversationData.EscalationRecord = await Escalate(sendTurnContext, handoffEvent).ConfigureAwait(false);
                    await _conversationState.SaveChangesAsync(turnContext).ConfigureAwait(false);
                }

                // run full pipeline
                var responses = await nextSend().ConfigureAwait(false);
                return responses;
            });

            await next(cancellationToken).ConfigureAwait(false);
        }

        private Task<LivePersonConversationRecord> Escalate(ITurnContext turnContext, IEventActivity handoffEvent)
        {
            var account = _creds.LpAccount;
            var clientId = _creds.LpAppId;
            var clientSecret = _creds.LpAppSecret;

            return LivePersonConnector.EscalateToAgenAsync(turnContext, handoffEvent, account, clientId, clientSecret, _conversationMap);
        }
    }
}
