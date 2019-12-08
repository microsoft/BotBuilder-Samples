// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples.DialogRootBot.Extensions
{
    public static class TurnContextEx
    {
        public static async Task SendTraceActivityAsync(this ITurnContext turnContext, string label = "Trace", string name = "Trace Message", object value = null, CancellationToken cancellationToken = default)
        {
            var traceActivity = new Activity(ActivityTypes.Trace)
            {
                Label = label,
                Name = name,
                Value = value
            };

            await turnContext.SendActivityAsync(traceActivity, cancellationToken);
        }
    }
}
