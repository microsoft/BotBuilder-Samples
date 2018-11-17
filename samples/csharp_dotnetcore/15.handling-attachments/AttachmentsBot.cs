// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// Represents a bot that processes incoming activities.
    /// For each user interaction, an instance of this class is created and the OnTurnAsync method is called.
    /// This is a Transient lifetime service. Transient lifetime services are created
    /// each time they're requested. For each Activity received, a new instance of this
    /// class is created. Objects that are expensive to construct, or have a lifetime
    /// beyond the single turn, should be carefully managed.
    /// For example, the <see cref="MemoryStorage"/> object and associated
    /// <see cref="IStatePropertyAccessor{T}"/> object are created with a singleton lifetime.
    /// This bot responds to the user's text input with an <see cref="Attachment"/> (in this example, an image)
    /// using various types of attachments. In this case, we are displaying an image from a file on the server,
    /// an image from an HTTP URL, and an uploaded image. In this project the user also has the option to upload
    /// an <see cref="Attachment"/> to the bot. The <see cref="Attachment"/> saves to the same directory as the
    /// AttachmentBot.cs file. A message exchange between user and bot may contain media attachments, such as images,
    /// video, audio, and files. The types of attachments that can be sent and received varies by channel. In this
    /// sample we demonstrate sending a <see cref="HeroCard"/> and images as attachments. Also demonstrated is the
    /// ability of a bot to receive file attachments.
    /// </summary>
    /// <seealso cref="https://docs.microsoft.com/en-us/aspnet/core/fundamentals/dependency-injection?view=aspnetcore-2.1"/>
    public class AttachmentsBot : IBot
    {
        /// <summary>
        /// Every conversation turn for our Echo Bot will call this method.
        /// There are no dialogs used, since it's "single turn" processing, meaning a single
        /// request and response.
        /// </summary>
        /// <param name="turnContext">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn. </param>
        /// <param name="cancellationToken">(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
        /// <seealso cref="BotStateSet"/>
        /// <seealso cref="ConversationState"/>
        /// <seealso cref="IMiddleware"/>
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (turnContext == null)
            {
                throw new ArgumentNullException(nameof(turnContext));
            }

            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                // Take the input from the user and create the appropriate response.
                var reply = ProcessInput(turnContext);

                // Respond to the user.
                await turnContext.SendActivityAsync(reply, cancellationToken);

                await DisplayOptionsAsync(turnContext, cancellationToken);
            }
            else if (turnContext.Activity.Type == ActivityTypes.ConversationUpdate)
            {
                if (turnContext.Activity.MembersAdded != null)
                {
                    // Send a welcome message to the user and tell them what actions they may perform to use this bot
                    await SendWelcomeMessageAsync(turnContext, cancellationToken);
                }
            }
            else
            {
                // Default behavior for all other type of activities.
                await turnContext.SendActivityAsync($"{turnContext.Activity.Type} activity detected");
            }
        }

        /// <summary>
        ///  Displays a <see cref="HeroCard"/> with options for the user to select.
        /// </summary>
        /// <param name="turnContext">Provides the <see cref="ITurnContext"/> for the turn of the bot.</param>
        /// <param name="cancellationToken" >(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the operation result of the operation.</returns>
        private static async Task DisplayOptionsAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var reply = turnContext.Activity.CreateReply();

            // Create a HeroCard with options for the user to interact with the bot.
            var card = new HeroCard
            {
                Text = "You can upload an image or select one of the following choices",
                Buttons = new List<CardAction>
                {
                    // Note that some channels require different values to be used in order to get buttons to display text.
                    // In this code the emulator is accounted for with the 'title' parameter, but in other channels you may
                    // need to provide a value for other parameters like 'text' or 'displayText'.
                    new CardAction(ActionTypes.ImBack, title: "1. Inline Attachment", value: "1"),
                    new CardAction(ActionTypes.ImBack, title: "2. Internet Attachment", value: "2"),
                    new CardAction(ActionTypes.ImBack, title: "3. Uploaded Attachment", value: "3"),
                },
            };

            // Add the card to our reply.
            reply.Attachments = new List<Attachment>() { card.ToAttachment() };

            await turnContext.SendActivityAsync(reply, cancellationToken);
        }

        /// <summary>
        /// Greet the user and give them instructions on how to interact with the bot.
        /// </summary>
        /// <param name="turnContext">Provides the <see cref="ITurnContext"/> for the turn of the bot.</param>
        /// <param name="cancellationToken" >(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the operation result of the operation.</returns>
        private static async Task SendWelcomeMessageAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in turnContext.Activity.MembersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(
                        $"Welcome to AttachmentsBot {member.Name}." +
                        $" This bot will introduce you to Attachments." +
                        $" Please select an option",
                        cancellationToken: cancellationToken);
                    await DisplayOptionsAsync(turnContext, cancellationToken);
                }
            }
        }

        /// <summary>
        /// Given the input from the message <see cref="Activity"/>, create the response.
        /// </summary>
        /// <param name="turnContext">Provides the <see cref="ITurnContext"/> for the turn of the bot.</param>
        /// <returns>An <see cref="Activity"/> to send as a response.</returns>
        private static Activity ProcessInput(ITurnContext turnContext)
        {
            var activity = turnContext.Activity;
            var reply = activity.CreateReply();

            if (activity.Attachments != null && activity.Attachments.Any())
            {
                // We know the user is sending an attachment as there is at least one item
                // in the Attachments list.
                HandleIncomingAttachment(activity, reply);
            }
            else
            {
                // Send at attachment to the user.
                HandleOutgoingAttachment(activity, reply);
            }

            return reply;
        }

        /// <summary>
        /// Adds an attachment to the 'reply' parameter that is passed in.
        /// </summary>
        private static void HandleOutgoingAttachment(IMessageActivity activity, IMessageActivity reply)
        {
            // Look at the user input, and figure out what kind of attachment to send.
            if (activity.Text.StartsWith("1"))
            {
                reply.Text = "This is an inline attachment.";
                reply.Attachments = new List<Attachment>() { GetInlineAttachment() };
            }
            else if (activity.Text.StartsWith("2"))
            {
                reply.Text = "This is an attachment from a HTTP URL.";
                reply.Attachments = new List<Attachment>() { GetInternetAttachment() };
            }
            else if (activity.Text.StartsWith("3"))
            {
                reply.Text = "This is an uploaded attachment.";

                // Get the uploaded attachment.
                var uploadedAttachment = GetUploadedAttachmentAsync(reply.ServiceUrl, reply.Conversation.Id).Result;
                reply.Attachments = new List<Attachment>() { uploadedAttachment };
            }
            else
            {
                // The user did not enter input that this bot was built to handle.
                reply.Text = "Your input was not recognized please try again.";
            }
        }

        /// <summary>
        /// Handle attachments uploaded by users. The bot receives an <see cref="Attachment"/> in an <see cref="Activity"/>.
        /// The activity has a <see cref="IList{T}"/> of attachments.
        /// </summary>
        /// <remarks>
        /// Not all channels allow users to upload files. Some channels have restrictions
        /// on file type, size, and other attributes. Consult the documentation for the channel for
        /// more information. For example Skype's limits are here
        /// <see ref="https://support.skype.com/en/faq/FA34644/skype-file-sharing-file-types-size-and-time-limits"/>.
        /// </remarks>
        private static void HandleIncomingAttachment(IMessageActivity activity, IMessageActivity reply)
        {
            foreach (var file in activity.Attachments)
            {
                // Determine where the file is hosted.
                var remoteFileUrl = file.ContentUrl;

                // Save the attachment to the system temp directory.
                var localFileName = Path.Combine(Path.GetTempPath(), file.Name);

                // Download the actual attachment
                using (var webClient = new WebClient())
                {
                    webClient.DownloadFile(remoteFileUrl, localFileName);
                }

                reply.Text = $"Attachment \"{activity.Attachments[0].Name}\"" +
                             $" has been received and saved to \"{localFileName}\"";
            }
        }

        /// <summary>
        /// Creates an inline attachment sent from the bot to the user using a base64 string.
        /// </summary>
        /// <returns>An <see cref="Attachment"/> to be displayed to the user.</returns>
        /// <remarks>
        /// Using a base64 string to send an attachment will not work on all channels.
        /// Additionally, some channels will only allow certain file types to be sent this way.
        /// For example a .png file may work but a .pdf file may not on some channels.
        /// Please consult the channel documentation for specifics.
        /// </remarks>
        private static Attachment GetInlineAttachment()
        {
            var imagePath = Path.Combine(Environment.CurrentDirectory, @"Resources\architecture-resize.png");
            var imageData = Convert.ToBase64String(File.ReadAllBytes(imagePath));

            return new Attachment
            {
                Name = @"Resources\architecture-resize.png",
                ContentType = "image/png",
                ContentUrl = $"data:image/png;base64,{imageData}",
            };
        }

        /// <summary>
        /// Creates an <see cref="Attachment"/> to be sent from the bot to the user from an uploaded file.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the operation result.</returns>
        private static async Task<Attachment> GetUploadedAttachmentAsync(string serviceUrl, string conversationId)
        {
            if (string.IsNullOrWhiteSpace(serviceUrl))
            {
                throw new ArgumentNullException(nameof(serviceUrl));
            }

            if (string.IsNullOrWhiteSpace(conversationId))
            {
                throw new ArgumentNullException(nameof(conversationId));
            }

            var imagePath = Path.Combine(Environment.CurrentDirectory, @"Resources\architecture-resize.png");

            // Create a connector client to use to upload the image.
            using (var connector = new ConnectorClient(new Uri(serviceUrl)))
            {
                var attachments = new Attachments(connector);
                var response = await attachments.Client.Conversations.UploadAttachmentAsync(
                    conversationId,
                    new AttachmentData
                    {
                        Name = @"Resources\architecture-resize.png",
                        OriginalBase64 = File.ReadAllBytes(imagePath),
                        Type = "image/png",
                    });

                var attachmentUri = attachments.GetAttachmentUri(response.Id);

                return new Attachment
                {
                    Name = @"Resources\architecture-resize.png",
                    ContentType = "image/png",
                    ContentUrl = attachmentUri,
                };
            }
        }

        /// <summary>
        /// Creates an <see cref="Attachment"/> to be sent from the bot to the user from a HTTP URL.
        /// </summary>
        /// <returns>A <see cref="Task"/> representing the operation result.</returns>
        private static Attachment GetInternetAttachment()
        {
            // ContentUrl must be HTTPS.
            return new Attachment
            {
                Name = @"Resources\architecture-resize.png",
                ContentType = "image/png",
                ContentUrl = "https://docs.microsoft.com/en-us/bot-framework/media/how-it-works/architecture-resize.png",
            };
        }
    }
}
