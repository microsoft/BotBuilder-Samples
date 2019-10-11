// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class HelperBot : TeamsActivityHandler
    {
        /*
         * This bot should be installed within a team with at least 3 channels. You can @mention the bot "notify", "card", "random", or "general" to
         * have the message notify the user with a message, notify the user with a card, send a message to a random channel, or send a message to the 
         * general channel respectively.
         */
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            turnContext.Activity.RemoveRecipientMention();

            if (turnContext.Activity.Text == "notify")
            {
                var msg = MessageFactory.Text("This message will contain a notification");
                msg.TeamsNotifyUser();

                await turnContext.SendActivityAsync(msg, cancellationToken);
            }
            else if (turnContext.Activity.Text == "card")
            {
                var heroCard = new HeroCard(text: "You will receive a notification in your Activity feed.");
                var msg = MessageFactory.Text("hi");
                msg.Attachments = new List<Attachment> { heroCard.ToAttachment() };
                msg.Summary = "This text will show in the activity feed as preview text";
                msg.TeamsNotifyUser();

                await turnContext.SendActivityAsync(msg, cancellationToken);
            }
            else if (turnContext.Activity.Text == "random")
            {
                var channels = await TeamsInfo.GetChannelsAsync(turnContext, cancellationToken);
                Random random = new Random();
                var channel = random.Next(0, channels.Count);
                var channelName = channels[channel].Name == null ? "General" : channels[channel].Name;

                var msg = MessageFactory.Text($"I will send this to the {channelName} channel");

                await turnContext.TeamsSendToChannelAsync(channels[channel].Id, msg, cancellationToken);
                await turnContext.SendActivityAsync(msg, cancellationToken);
            }
            else if (turnContext.Activity.Text == "general")
            {
                var msg = MessageFactory.Text("This message will appear in the team's general channel");
                await turnContext.TeamsSendToGeneralChannelAsync(msg, cancellationToken);
            }
            else
            {
                var msg = MessageFactory.Text("Send me \"notify\" to get a notificaiton. Send me \"random\" to send a message to a random channel." +
                    "Send me \"general\" to send a message to the general channel. Send me \"card\" to receive a card notification.");
                await turnContext.SendActivityAsync(msg, cancellationToken);
            }
        }
    }
}
