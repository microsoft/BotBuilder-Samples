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
                    var reply = ProcessInput(turnContext);

                    // Respond to the user.
                    await turnContext.SendActivityAsync(reply, cancellationToken);

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
            var card = new HeroCard
            {
                Text = "You can upload an image or select one of the following choices",
                Buttons = new List<CardAction>()
                {
                    // Note that some channel require different values to be used in order to get buttons to display text.
                    // In this code the emulator is accounted for with 'title', but in other channels you may need to provide a value
                    // for other parameters like 'text' or 'displayText'.
                    new CardAction(ActionTypes.ImBack, title: "1. Inline Attachment", value: "1"),
                    new CardAction(ActionTypes.ImBack, title: "2. Internet Attachment", value: "2"),
                    new CardAction(ActionTypes.ImBack, title: "3. Uploaded Attachment", value: "3"),
                },
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
        /// <returns>A <see cref="Activity"/> to send as a response to the user's input.</returns>
        private static Activity ProcessInput(ITurnContext turnContext)
        {
            var activity = turnContext.Activity;
            var reply = activity.CreateReply();

            if (activity.Attachments != null && activity.Attachments.Any())
            {
                HandleIncomingAttachment(activity, reply);
            }
            else
            {
                HandleOutgoingAttachment(activity, reply);
            }

            return reply;
        }

        /// <summary>
        /// This method responds with an <see cref="Activity"/> that demonstrates the use of the type
        /// of attachment the user requested.
        /// </summary>
        private static void HandleOutgoingAttachment(Activity activity, Activity reply)
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
                reply.Text = "This is an uploaded attachment";
                reply.Attachments = new List<Attachment>() { GetUploadedAttachmentAsync(reply.ServiceUrl, reply.Conversation.Id).Result };
            }
            else
            {
                // The user did not enter input that this bot was built to handle.
                reply.Text = "Your input was not recognized please try again";
            }
        }

        /// <summary>
        /// Handles when the user uploads an attachment to the bot by saving the attachment to
        /// the directory this file resides in.
        /// </summary>
        /// <remarks>Not all channel allow users to upload files.  Some channels may have restrictions
        /// on file type, size, and other attributes.  Consult the documentation for the channel for
        /// more information</remarks>
        private static void HandleIncomingAttachment(IMessageActivity activity, IMessageActivity reply)
        {
            foreach (var file in activity.Attachments)
            {
                // Where the file is hosted.
                var remoteFileUrl = file.ContentUrl;

                // Where we are saving the file.
                var localFileName = Path.Combine(Environment.CurrentDirectory, file.Name);

                // Save the file locally where the bot is hosted.
                using (var webClient = new WebClient())
                {
                    webClient.DownloadFile(remoteFileUrl, localFileName);
                }

                reply.Text = $"Attachment \"{activity.Attachments[0].Name}\"" +
                             $" has been recieved and saved to \"{localFileName}\"";
            }
        }

        /// <summary>
        /// Creates an inline attachment sent from the bot to the user using a base64 string.
        /// </summary>
        /// <returns>An <see cref="Attachment"/> to be displayed to the user.</returns>
        /// <remarks>Using a base64 string to send an attachment will not work on all channels.
        /// Additionally, some channels will only allow certain file types to be sent this way.
        /// For example a .png file may work but a .pdf file may not on some channels.
        /// Please consult the channel documentation for specifics.</remarks>
        private static Attachment GetInlineAttachment()
        {
            var imagePath = Path.Combine(Environment.CurrentDirectory, "architecture-resize.png");

            var imageData = Convert.ToBase64String(File.ReadAllBytes(imagePath));

            return new Attachment
            {
                Name = "architecture-resize.png",
                ContentType = "image/png",
                ContentUrl = $"data:image/png;base64,{imageData}",
            };
        }

        /// <summary>
        /// Creates an attachmment to be sent from the bot to the user from an uploaded file.
        /// </summary>
        /// <returns>>A <see cref="Task"/> representing the operation result of the Turn operation.</returns>
        private static async Task<Attachment> GetUploadedAttachmentAsync(string serviceUrl, string conversationId)
        {
            if (string.IsNullOrWhiteSpace(serviceUrl))
            {
                throw new ArgumentNullException();
            }

            var imagePath = Path.Combine(Environment.CurrentDirectory, "architecture-resize.png");

            // Create a connector client to use to upload the image.
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

        /// <summary>
        /// Creates an attachmment to be sent from the bot to the user from a https url.
        /// </summary>
        /// <returns>>A <see cref="Task"/> representing the operation result of the Turn operation.</returns>
        private static Attachment GetInternetAttachment()
        {
            // ContentUrl must be https.
            return new Attachment
            {
                Name = "architecture-resize.png",
                ContentType = "image/png",
                ContentUrl = "https://docs.microsoft.com/en-us/bot-framework/media/how-it-works/architecture-resize.png",
            };
        }
    }
}
