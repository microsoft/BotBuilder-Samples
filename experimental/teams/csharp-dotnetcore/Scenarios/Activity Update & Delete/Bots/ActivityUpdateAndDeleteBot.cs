// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

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
            if (_list.Count == 0)
            {
                await SendMessageAndLogActivityId(turnContext, "Hello. If you message me again I'll echo your message and update all sent messages with your text.", cancellationToken);
            }
            else
            {
                await SendMessageAndLogActivityId(turnContext, $"{turnContext.Activity.Text}", cancellationToken);
                foreach (var activityId in _list)
                {
                    var newActivity = new Activity
                    {
                        Text = turnContext.Activity.Text,
                        Id = activityId,
                        ChannelId = "msteams",
                        Conversation = turnContext.Activity.Conversation,
                        Type = "message"
                    };

                    await turnContext.UpdateActivityAsync(newActivity, cancellationToken);
                }
            }
        }

        private async Task SendMessageAndLogActivityId(ITurnContext turnContext, string text, CancellationToken cancellationToken)
        {
            // We need to record the Activity Id from the Activity just sent in order to understand what the reaction is a reaction too. 
            var replyActivity = MessageFactory.Text(text);
            var resourceResponse = await turnContext.SendActivityAsync(replyActivity, cancellationToken);
            _list.Add(resourceResponse.Id);
        }
    }
}
