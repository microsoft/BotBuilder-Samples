// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace Console_EchoBot
{
    public class EchoBot : IBot
    {
        // Every Conversation turn for our EchoBot will call this method. In here
        // the bot checks the <see cref="Activity"/> type to verify it's a <see cref="ActivityTypes.Message"/>
        // message, and then echoes the user's typing back to them.
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            // Handle Message activity type, which is the main activity type within a conversational interface
            // Message activities may contain text, speech, interactive cards, and binary or unknown attachments.
            // see https://aka.ms/about-bot-activity-message to learn more about the message and other activity types
            if (turnContext.Activity.Type == ActivityTypes.Message && !string.IsNullOrEmpty(turnContext.Activity.Text))
            {
                // Check to see if the user sent a simple "quit" message.
                if (turnContext.Activity.Text.Equals("quit", StringComparison.InvariantCultureIgnoreCase))
                {
                    // Send a reply.
                    await turnContext.SendActivityAsync($"Bye!", cancellationToken: cancellationToken);
                    System.Environment.Exit(0);
                }
                else
                {
                    // Echo back to the user whatever they typed.
                    await turnContext.SendActivityAsync($"You sent '{turnContext.Activity.Text}'", cancellationToken: cancellationToken);
                }
            }
            else
            {
                await turnContext.SendActivityAsync($"{turnContext.Activity.Type} event detected", cancellationToken: cancellationToken);
            }
        }
    }
}
