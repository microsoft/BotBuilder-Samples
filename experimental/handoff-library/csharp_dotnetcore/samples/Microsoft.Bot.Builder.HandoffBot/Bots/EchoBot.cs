// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace Microsoft.Bot.Builder.EchoBot
{
    public class EchoBot : ActivityHandlerWithHandoff
    {
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            if (turnContext.Activity.Text.Contains("human"))
            {
                await turnContext.SendActivityAsync($"You have requested transfer to an agent, ConversationId={turnContext.Activity.Conversation.Id}");

                var a1 = MessageFactory.Text($"first message");
                var a2 = MessageFactory.Text($"second message");
                var transcript = new Activity[] { a1, a2 };
                var context = new { Skill = "credit cards" };

                var handoffEvent = EventFactory.CreateHandoffInitiation(turnContext, context, new Transcript(transcript));
                await turnContext.SendActivityAsync(handoffEvent);

                await turnContext.SendActivityAsync($"Agent transfer has been initiated");

            }
            else
            {
                await turnContext.SendActivityAsync(MessageFactory.Text($"Echo: {turnContext.Activity.Text}"), cancellationToken);
            }
        }

        protected override async Task OnEventActivityAsync(ITurnContext<IEventActivity> turnContext, CancellationToken cancellationToken)
        {
            var evnt = turnContext.Activity;
            await turnContext.SendActivityAsync($"Received event Name='{evnt.Name}', Status='{evnt.Value}'");
            await base.OnEventActivityAsync(turnContext, cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Hello and welcome!"), cancellationToken);
                }
            }
        }
    }
}
