// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Builder.Teams.Internal;
using Microsoft.Bot.Builder.TraceExtensions;
using Microsoft.Bot.Connector.Authentication;
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
            var teamsContext = new TeamsContext(turnContext, null);
            string actualText = teamsContext.GetActivityTextWithoutMentions();

            if (_list.Count == 0)
            {
                var welcomeMessage = "Hello. If you message me again I'll echo your message and update all sent messages with your text. " +
                                     "Type \"delete\" to delete all messages sent";
                await SendMessageAndLogActivityId(turnContext, welcomeMessage, cancellationToken);
            }
            else if (actualText == "delete")
            {
                foreach (var activityId in _list)
                {
                    await turnContext.DeleteActivityAsync(activityId, cancellationToken);
                }
                _list.Clear();   
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
