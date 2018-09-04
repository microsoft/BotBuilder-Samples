// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;

namespace EnterpriseBot
{
    public class EventDebuggerMiddleware : IMiddleware
    {
        public async Task OnTurnAsync(ITurnContext turnContext, NextDelegate next, CancellationToken cancellationToken = default(CancellationToken))
        {
            var activity = turnContext.Activity;

            if (activity.Type == ActivityTypes.Message)
            {
                var text = turnContext.Activity.Text;

                if (text.StartsWith("/event:"))
                {
                    var json = text.Split("/event:")[1];
                    var body = JsonConvert.DeserializeObject<Activity>(json);

                    turnContext.Activity.Type = ActivityTypes.Event;
                    turnContext.Activity.Name = body.Name;
                    turnContext.Activity.Value = body.Value;
                }
            }

            await next(cancellationToken).ConfigureAwait(false);
        }
    }
}
