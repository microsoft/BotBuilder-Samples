// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;

namespace Microsoft.Bot.Builder.Adapters.Twilio.TestBot.Bots
{
    public class EchoBot : ActivityHandler
    {
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var activity = MessageFactory.Text($"Echo: {turnContext.Activity.Text}");
            if (turnContext.Activity.Attachments != null)
            {
                activity.Text += $" got {turnContext.Activity.Attachments.Count} attachments";
                for (var i = 0; i < turnContext.Activity.Attachments.Count; i++)
                {
                    var image = new Attachment("image/png", "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQtB3AwMUeNoq4gUBGe6Ocj8kyh3bXa9ZbV7u1fVKQoyKFHdkqU");
                    activity.Attachments.Add(image);
                }
            }

            await turnContext.SendActivityAsync(activity, cancellationToken);
        }
    }
}