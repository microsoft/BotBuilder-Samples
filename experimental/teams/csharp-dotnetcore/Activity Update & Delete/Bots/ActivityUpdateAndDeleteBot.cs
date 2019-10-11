// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples.Bots
{
    /*
     * From the UI you can just @mention the bot from any channelwith any string EXCEPT for "delete". if you send the bot "delete" it will delete
     * all of the previous bot responses and empty it's internal storage.
     */
    public class ActivityUpdateAndDeleteBot : ActivityHandler
    {
        private List<string> _list;

        public ActivityUpdateAndDeleteBot(List<string> list)
        {
            _list = list;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            turnContext.Activity.RemoveRecipientMention();
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
                    var newActivity = MessageFactory.Text(turnContext.Activity.Text);
                    newActivity.Id = activityId;
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
