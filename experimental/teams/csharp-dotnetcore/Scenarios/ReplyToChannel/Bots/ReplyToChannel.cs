// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class ActivityUpdateAndDeleteBot : TeamsActivityHandler
    {
        private List<string> _list;

        public ActivityUpdateAndDeleteBot(List<string> list)
        {
            _list = list;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var message = $"You said {turnContext.Activity.Text}";
            await SendMessageAndLogActivityId(turnContext, message, cancellationToken);
    
        }

        private async Task SendMessageAndLogActivityId(ITurnContext turnContext, string text, CancellationToken cancellationToken)
        {
            var replyActivity = MessageFactory.Text(text);
            replyActivity.ApplyConversationReference(turnContext.Activity.GetConversationReference());

            // There are 2 ways to send a message to a channel. You can get the channelID from the TeamsChannelData object, or you can pull the
            // Channel ID from the conversationId after you drop the message ID that's packaged with the channel ID from Teams. Below are the
            // two ways you can do this.

            // var channelId = turnContext.Activity.GetChannelData<TeamsChannelData>().Channel.Id;
            var channelId = turnContext.Activity.Conversation.Id.Split(";")[0];
            replyActivity.Conversation.Id = channelId;
            var resourceResponse = await turnContext.SendActivityAsync(replyActivity, cancellationToken);
            _list.Add(resourceResponse.Id);
        }
    }
}
