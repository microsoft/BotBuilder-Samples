// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;

namespace LivePersonAgentBot
{
    public class HandoffMiddleware : IMiddleware
    {
        public HandoffMiddleware()
        {
        }

        public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default)
        {
            turnContext.OnSendActivities(async (sendTurnContext, activities, nextSend) =>
            {
                Activity handOffEventActivity = activities.Where((activity) => activity.Type == ActivityTypes.Event &&
                                                                               activity.Name == HandoffEventNames.InitiateHandoff).FirstOrDefault();

                if (handOffEventActivity != null)
                {
                    var context = handOffEventActivity.Value as JObject;

                    var skill = context?.Value<string>("Skill");
                    if(String.IsNullOrEmpty(skill))
                    {
                        skill = "default";
                    }

                    var channelData = new { action =
                        new {
                            name = "TRANSFER",
                            parameters = new { skill }
                        }
                    };

                    var activity = new Activity("message");
                    activity.Text = "";
                    activity.ChannelData = channelData;

                    await sendTurnContext.SendActivityAsync(activity).ConfigureAwait(false);
                }

                // run full pipeline
                var responses = await nextSend().ConfigureAwait(false);
                return responses;
            });

            await next(cancellationToken).ConfigureAwait(false);
        }
    }
}
