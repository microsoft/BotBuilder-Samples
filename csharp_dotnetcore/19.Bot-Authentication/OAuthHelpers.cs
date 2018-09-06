// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Attachment = Microsoft.Bot.Schema.Attachment;

namespace Bot_Authentication
{
    // This class is where we make the calls to the Graph API. The following scopes are used:
    // 'OpenId' 'email' 'Mail.Send.Shared' 'Mail.Read' 'profile' 'User.Read' 'User.ReadBasic.All'
    // for more information about scopes see:
    // https://developer.microsoft.com/en-us/graph/docs/concepts/permissions_reference
    public static class OAuthHelpers
    {
        // Allows the user to send an email from the bot on their behalf.
        public static async Task SendMailAsync(ITurnContext context, TokenResponse tokenResponse, string recipient)
        {
            var token = tokenResponse;
            var client = new SimpleGraphClient(token.Token);

            var me = await client.GetMeAsync();

            await client.SendMailAsync(recipient, "Message from a bot!", $"Hi there! I had this message sent from a bot. - Your friend, {me.DisplayName}");

            await context.SendActivityAsync($"I sent a message to '{recipient}' from your account :)");
        }

        // Displays information about the user in the bot
        public static async Task ListMeAsync(ITurnContext context, TokenResponse tokenResponse)
        {
            var token = tokenResponse;
            var client = new SimpleGraphClient(token.Token);

            var me = await client.GetMeAsync();
            var manager = await client.GetManagerAsync();

            var reply = context.Activity.CreateReply();
            var photoResponse = await client.GetPhotoAsync();
            var photoText = string.Empty;
            if (photoResponse != null)
            {
                var replyAttachment = new Attachment(photoResponse.ContentType, photoResponse.Base64String);
                reply.Attachments.Add(replyAttachment);
            }
            else
            {
                photoText = "You should really add an image to your Outlook profile :)";
            }

            reply.Text = $"You are {me.DisplayName} and you report to {manager.DisplayName}.  {photoText}";
            await context.SendActivityAsync(reply);
        }

        // Gets recent mail the user has received within the last hour and displays up
        // to 5 of the emails in the bot.
        public static async Task ListRecentMailAsync(ITurnContext context, TokenResponse tokenResponse)
        {
            var client = new SimpleGraphClient(tokenResponse.Token);
            var messages = await client.GetRecentUnreadMailAsync();
            var reply = context.Activity.CreateReply();

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
                reply.Text = "Unable to find any unread mail in the past 60 minutes";
            }

            await context.SendActivityAsync(reply);
        }

        // Prompts the user to log in using the OAuth provider specified by the connection name.
        public static OAuthPrompt Prompt(string connectionName)
        {
            return new OAuthPrompt(
                "loginPrompt",
                new OAuthPromptSettings
                {
                    ConnectionName = connectionName,
                    Text = "Please Sign In",
                    Title = "Sign In",
                    Timeout = 300000, // User has 5 minutes to login
                });
        }
    }
}
