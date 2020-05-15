using System;
using Microsoft.Bot.Builder;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Configuration;
using System.Linq;
using LivePersonAgentBot.Bots;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using System.Collections.Concurrent;
using Newtonsoft.Json.Linq;

namespace LivePersonAgentBot
{
    public class HandoffMiddleware : Microsoft.Bot.Builder.IMiddleware
    {
        public HandoffMiddleware()
        {
        }

        public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default)
        {
            turnContext.OnSendActivities(async (sendTurnContext, activities, nextSend) =>
            {
                Activity handOffEventActivity = activities.Where((activity) => activity.Type == ActivityTypes.Event && activity.Name == HandoffEventNames.InitiateHandoff).FirstOrDefault();

                if (handOffEventActivity != null)
                {
                    var context = handOffEventActivity.Value as JObject;

                    var skill = context?.Value<string>("Skill");
                    if(String.IsNullOrEmpty(skill))
                    {
                        skill = "default";
                    }
#if false // using string literal - works but more verbose
                    var channelData = @"
                    {
                    ""action"": {
                        ""name"": ""TRANSFER"",
                        ""parameters"": {
                            ""skill"": ""credit cards""
                        }
                    }
                    }";
                    var activity = new Activity("message");
                    activity.Text = "";
                    activity.ChannelData = JObject.Parse(channelData);
#else // nicer
                    var channelData = new { action =
                        new { name = "TRANSFER"},
                        parameters = new { skill }
                    };

                    var activity = new Activity("message");
                    activity.Text = "";
                    activity.ChannelData = channelData;
#endif

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
