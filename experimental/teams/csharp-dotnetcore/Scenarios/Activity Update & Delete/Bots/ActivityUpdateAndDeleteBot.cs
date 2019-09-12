// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
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
            foreach (var mention in turnContext.Activity.GetMentions().Where(mention => mention.Mentioned.Id == turnContext.Activity.Recipient.Id))

            {
                if (mention.Text == null)
                {
                    turnContext.Activity.Text = Regex.Replace(turnContext.Activity.Text, "<at>" + mention.Mentioned.Name + "</at>", string.Empty, RegexOptions.IgnoreCase).Trim();
                }
                else
                {
                    turnContext.Activity.Text = Regex.Replace(turnContext.Activity.Text, mention.Text, string.Empty, RegexOptions.IgnoreCase).Trim();
                }
            }

            

          if (turnContext.Activity.Text == "delete")
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
            replyActivity.ApplyConversationReference(turnContext.Activity.GetConversationReference());
            var resourceResponse = await turnContext.SendActivityAsync(replyActivity, cancellationToken);
            _list.Add(resourceResponse.Id);
        }
    }
}
