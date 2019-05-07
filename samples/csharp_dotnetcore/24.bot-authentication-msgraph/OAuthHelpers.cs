// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Attachment = Microsoft.Bot.Schema.Attachment;

namespace Microsoft.BotBuilderSamples
{
    // This class calls the Microsoft Graph API. The following OAuth scopes are used:
    // 'OpenId' 'email' 'Mail.Send.Shared' 'Mail.Read' 'profile' 'User.Read' 'User.ReadBasic.All'
    // for more information about scopes see:
    // https://developer.microsoft.com/en-us/graph/docs/concepts/permissions_reference
    public static class OAuthHelpers
    {
        // Enable the user to send an email via the bot.
        public static async Task SendMailAsync(ITurnContext turnContext, TokenResponse tokenResponse, string recipient)
        {
            if (turnContext == null)
            {
                throw new ArgumentNullException(nameof(turnContext));
            }

            if (tokenResponse == null)
            {
                throw new ArgumentNullException(nameof(tokenResponse));
            }

            var client = new SimpleGraphClient(tokenResponse.Token);
            var me = await client.GetMeAsync();

            await client.SendMailAsync(
                recipient,
                "Message from a bot!",
                $"Hi there! I had this message sent from a bot. - Your friend, {me.DisplayName}");

            await turnContext.SendActivityAsync(
                $"I sent a message to '{recipient}' from your account.");
        }

        // Displays information about the user in the bot.
        public static async Task ListMeAsync(ITurnContext turnContext, TokenResponse tokenResponse)
        {
            if (turnContext == null)
            {
                throw new ArgumentNullException(nameof(turnContext));
            }

            if (tokenResponse == null)
            {
                throw new ArgumentNullException(nameof(tokenResponse));
            }

            // Pull in the data from the Microsoft Graph.
            var client = new SimpleGraphClient(tokenResponse.Token);
            var me = await client.GetMeAsync();

            await turnContext.SendActivityAsync($"You are {me.DisplayName}.");
        }

        // Gets recent mail the user has received within the last hour and displays up
        // to 5 of the emails in the bot.
        public static async Task ListRecentMailAsync(ITurnContext turnContext, TokenResponse tokenResponse)
        {
            if (turnContext == null)
            {
                throw new ArgumentNullException(nameof(turnContext));
            }

            if (tokenResponse == null)
            {
                throw new ArgumentNullException(nameof(tokenResponse));
            }

            var client = new SimpleGraphClient(tokenResponse.Token);
            var messages = await client.GetRecentMailAsync();
            var reply = turnContext.Activity.CreateReply();

            if (messages.Any())
            {
                var count = messages.Length;
                if (count > 5)
                {
                    count = 5;
                }

                reply.Attachments = new List<Attachment>();
                reply.AttachmentLayout = AttachmentLayoutTypes.Carousel;

                for (var i = 0; i < count; i++)
                {
                    var mail = messages[i];
                    var card = new HeroCard(
                        mail.Subject,
                        $"{mail.From.EmailAddress.Name} <{mail.From.EmailAddress.Address}>",
                        mail.BodyPreview,
                        new List<CardImage>()
                        {
                            new CardImage(
                                "https://botframeworksamples.blob.core.windows.net/samples/OutlookLogo.jpg",
                                "Outlook Logo"),
                        });
                    reply.Attachments.Add(card.ToAttachment());
                }
            }
            else
            {
                reply.Text = "Unable to find any recent unread mail.";
            }

            await turnContext.SendActivityAsync(reply);
        }
    }
}
