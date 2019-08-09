// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Builder.Handoff;

namespace Microsoft.Bot.Builder.EchoBot
{
    public class EchoBot : ActivityHandlerWithHandoff
    {
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            if (turnContext.Activity.Text.Contains("human"))
            {
                await turnContext.SendActivityAsync("You will be transferred to a human agent. Sit tight.");

                var a1 = MessageFactory.Text($"first message");
                var a2 = MessageFactory.Text($"second message");
                var transcript = new Activity[] { a1, a2 };
                var context = new { Skill = "credit cards", MSCallerId = "CCI" };
                var request = await turnContext.InitiateHandoffAsync(transcript, context, cancellationToken);

                if (await request.IsCompletedAsync())
                {
                    await turnContext.SendActivityAsync("Handoff request has been completed");
                }
                else
                {
                    await turnContext.SendActivityAsync("Handoff request has NOT been completed");
                }
            }
            else
            {
                await turnContext.SendActivityAsync(MessageFactory.Text($"Echo: {turnContext.Activity.Text}"), cancellationToken);
            }
        }

        protected override async Task OnHandoffActivityAsync(ITurnContext<IHandoffActivity> turnContext, CancellationToken cancellationToken)
        {
            var conversationId = turnContext.Activity.Conversation.Id;
            await turnContext.SendActivityAsync($"Received Handoff ack for conversation {conversationId}");
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
