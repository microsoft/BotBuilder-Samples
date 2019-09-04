// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class ConversationUpdateBot : TeamsActivityHandler
    {
        protected async override Task OnChannelRenamedEventAsync(TeamsChannelData channelData, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            var reply = ((Activity)turnContext.Activity).CreateReply();

            var heroCard = new HeroCard
            {
                Text = $"{channelData.Channel.Name} is the new Channel name"

            };

            reply.Attachments.Add(heroCard.ToAttachment());
            await turnContext.SendActivityAsync(reply, cancellationToken);
        }
    }
}
