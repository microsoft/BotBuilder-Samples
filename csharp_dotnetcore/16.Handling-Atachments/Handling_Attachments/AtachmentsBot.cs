// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using System.IO;
using System.Net;
using Microsoft.Bot.Connector;

namespace Handling_Atachments
{
    /// <summary>
    /// This bot will respond to the user's text input with an <see cref="Attachment"/> (in this case an image)
    /// using various types of attachments.  In this case, we are displaying an image from a file on the server,
    /// an image from an https url, and an uploaded image.  In this project the user also has the option to upload
    /// an attachment to the bot.  The attachment will be saved to the the same directory as this file.
    /// A message exchange between user and bot can contain media attachments, such as images, video, audio, and files.
    /// The types of attachments that can be sent and recieved varies by channel.  In this sample we demonstrate sending
    /// a <see cref="HeroCard"/> and images as attachments.  Also demonstrated is the ability of a bot to recieve file attachments.
    /// </summary>
    public class AtachmentsBot : IBot
    {
        /// <summary>
        /// This controls what happens when an activity gets sent to the bot.
        /// </summary>
        /// <param name="turnContext">Provides the <see cref="ITurnContext"/> for the turn of the bot.</param>
        /// <param name="cancellationToken" >(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>>A <see cref="Task"/> representing the operation result of the Turn operation.</returns>
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (turnContext == null)
            {
                throw new ArgumentNullException(nameof(turnContext));
            }

            switch (turnContext.Activity.Type)
            {
                case ActivityTypes.Message:

                    // Take the input from the user and create the appropriate response.
                    var response = ProcessInputAsync(turnContext, cancellationToken);

                    // Respond to the user.
                    await turnContext.SendActivityAsync(response.Result, cancellationToken);

                    await DisplayOptionsAsync(turnContext, cancellationToken);

                    break;
                case ActivityTypes.ConversationUpdate:
                    // Send a welcome message to the user and tell them what actions they may perform to use this bot
                    if (turnContext.Activity.MembersAdded.Any())
                    {
                        await SendWelcomeMessageAsync(turnContext, cancellationToken);
                    }

                    break;
            }
        }

        private static async Task DisplayOptionsAsync(ITurnContext turnContext, CancellationToken cancellationToke)
        {
            var reply = turnContext.Activity.CreateReply();

            // Create a HeroCard with options for the user to choose to interact with the bot.
            var card = new HeroCard();
            card.Text = "You can upload an image or select one of the following choices";
            card.Buttons = new List<CardAction>()
            {
                new CardAction(ActionTypes.ImBack, title: "1. Inline Attachment", value: "1"),
                new CardAction(ActionTypes.ImBack, title: "2. Internet Attachment", value: "2"),
                new CardAction(ActionTypes.ImBack, title: "3. Uploaded Attachment", value: "3"),
            };

            // Add the card to our reply.
            reply.Attachments = new List<Attachment>() { card.ToAttachment() };

            await turnContext.SendActivityAsync(reply, cancellationToke);
        }

        /// <summary>
        /// On a conversation update activity sent to the bot, the bot will
        /// send a message to the any new user(s) that were added.
        /// </summary>
        /// <param name="turnContext">Provides the <see cref="ITurnContext"/> for the turn of the bot.</param>
        /// <param name="cancellationToken" >(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>>A <see cref="Task"/> representing the operation result of the Turn operation.</returns>
        private static async Task SendWelcomeMessageAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in turnContext.Activity.MembersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(
                        $"Welcome to AttachmentsBot {member.Name}." +
                        $" This bot will introduce you to Attachments." +
                        $"  Please select an option",
                        cancellationToken: cancellationToken);
                    await DisplayOptionsAsync(turnContext, cancellationToken);
                }
            }
        }

        /// <summary>
        /// Given the input from the message activity the user sent, create the appropriate response.
        /// </summary>
        /// <param name="turnContext">Provides the <see cref="ITurnContext"/> for the turn of the bot.</param>
        /// <param name="cancellationToken" >(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the operation result of the Turn operation.</returns>
        private static async Task<Activity> ProcessInputAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var activity = turnContext.Activity;
            var reply = activity.CreateReply();

            if (activity.Attachments != null && activity.Attachments.Any())
            {
                foreach (var file in activity.Attachments)
                {
                    // Where the file is hosted
                    var remoteFileUrl = file.ContentUrl;

                    // Where we are saving the file
                    var localFileName = Path.Combine(Environment.CurrentDirectory, file.Name);

                    using (var webClient = new WebClient())
                    {
                        webClient.DownloadFile(remoteFileUrl, localFileName);
                    }

                    reply.Text = $"Attachment \"{activity.Attachments[0].Name}\"" +
                                 $" has been recieved and saved to \"{localFileName}\"";
                }
            }
            else
            {
                if (activity.Text.StartsWith("1"))
                {
                    reply.Text = "This is an inline attachment";
                    reply.Attachments = new List<Attachment>() { GetInlineAttachment() };
                }
                else if (activity.Text.StartsWith("2"))
                {
                    reply.Text = "This is an attachment from a https url";
                    reply.Attachments = new List<Attachment>() { GetInternetAttachment() };
                }
                else if (activity.Text.StartsWith("3"))
                {
                    reply.Text = "This is an attachment from a  https url";
                    reply.Attachments = new List<Attachment>() { GetUploadedAttachmentAsync(reply.ServiceUrl, reply.Conversation.Id).Result };
                }
                else
                {
                    reply.Text = "Your input was not recognized please try again";
                    await DisplayOptionsAsync(turnContext, cancellationToken);
                }
            }

            return reply;
        }

        private static Attachment GetInlineAttachment()
        {
            var imagePath = Path.Combine(Environment.CurrentDirectory, "architecture-resize.png");

            var imageData = Convert.ToBase64String(File.ReadAllBytes(imagePath));

            return new Attachment
            {
                Name = "small-image.png",
                ContentType = "image/png",
                ContentUrl = $"data:image/png;base64,{imageData}",
            };
        }

        private static async Task<Attachment> GetUploadedAttachmentAsync(string serviceUrl, string conversationId)
        {
            var imagePath = Path.Combine(Environment.CurrentDirectory, "architecture-resize.png");

            using (var connector = new ConnectorClient(new Uri(serviceUrl)))
            {
                var attachments = new Attachments(connector);
                var response = await attachments.Client.Conversations.UploadAttachmentAsync(
                    conversationId,
                    new AttachmentData
                    {
                        Name = "architecture-resize.png",
                        OriginalBase64 = File.ReadAllBytes(imagePath),
                        Type = "image/png",
                    });

                var attachmentUri = attachments.GetAttachmentUri(response.Id);

                return new Attachment
                {
                    Name = "architecture-resize.png",
                    ContentType = "image/png",
                    ContentUrl = attachmentUri,
                };
            }
        }

        private static Attachment GetInternetAttachment()
        {
            return new Attachment
            {
                Name = "BotFrameworkOverview.png",
                ContentType = "image/png",
                ContentUrl = "https://docs.microsoft.com/en-us/bot-framework/media/how-it-works/architecture-resize.png",
            };
        }
    }
}
