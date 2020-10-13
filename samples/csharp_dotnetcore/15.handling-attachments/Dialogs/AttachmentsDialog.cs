// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Adaptive;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Actions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Conditions;
using Microsoft.Bot.Builder.Dialogs.Adaptive.Generators;
using Microsoft.Bot.Builder.LanguageGeneration;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples.Dialogs
{
    public class AttachmentsDialog : AdaptiveDialog
    {
        private readonly Templates _templates;

        public AttachmentsDialog() : base(nameof(AttachmentsDialog))
        {
            string[] paths = { ".", "Dialogs", $"AttachmentsDialog.lg" };
            var fullPath = Path.Combine(paths);
            _templates = Templates.ParseFile(fullPath);

            Triggers = new List<OnCondition>
            {
                // Add a rule to welcome user
                new OnConversationUpdateActivity()
                {
                    Actions = WelcomeUserSteps()
                },
                new OnUnknownIntent
                {
                    Actions = AttachmentActions()
                }
            };

            Generator = new TemplateEngineLanguageGenerator(_templates);
        }

        private List<Dialog> AttachmentActions()
        {
            return new List<Dialog>()
            {
                new IfCondition
                {
                    // If attachments are present from the user, retrieve them
                    // and notify the user.
                    Condition = "turn.activity.attachments != null && turn.activity.attachments.count > 0",
                    Actions = new List<Dialog>
                    {
                        new CodeAction(HandleIncomingAttachmentAsync),
                    },
                    ElseActions = new List<Dialog>
                    {
                        new SwitchCondition
                        {
                            Condition = "turn.activity.text",
                            Cases = new List<Case>
                            {
                                new Case("1", new List<Dialog>() { new CodeAction(GetInlineAttachmentAsync), new SendActivity("${InlineAttachmentMessage()}")}),
                                new Case("2", new List<Dialog>() { new SendActivity("${InternetAttachmentMessage()}") }),
                                new Case("3", new List<Dialog>() { new CodeAction(UploadAttachmentAsync), new SendActivity("${UploadAttachmentMessage()}")}),
                            },
                        }
                    }
                },
                new SendActivity("${AttachmentOptionsMessage()}")
            };
        }

        // Handle attachments uploaded by users. The bot receives an <see cref="Attachment"/> in an <see cref="Activity"/>.
        // The activity has a "IList{T}" of attachments.    
        // Not all channels allow users to upload files. Some channels have restrictions
        // on file type, size, and other attributes. Consult the documentation for the channel for
        // more information. For example Skype's limits are here
        // <see ref="https://support.skype.com/en/faq/FA34644/skype-file-sharing-file-types-size-and-time-limits"/>.
        private async Task<DialogTurnResult> HandleIncomingAttachmentAsync(DialogContext dc, Object options)
        {
            string replyText = "";
            foreach (var file in dc.Context.Activity.Attachments)
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

                replyText += $"Attachment \"{file.Name}\"" +
                             $" has been received and saved to \"{localFileName}\"\r\n";
            }

            await dc.Context.SendActivityAsync(replyText).ConfigureAwait(false);
            return await dc.EndDialogAsync().ConfigureAwait(false);
        }

        // Creates an inline attachment sent from the bot to the user using a base64 string.
        // Using a base64 string to send an attachment will not work on all channels.
        // Additionally, some channels will only allow certain file types to be sent this way.
        // For example a .png file may work but a .pdf file may not on some channels.
        // Please consult the channel documentation for specifics.
        private async Task<DialogTurnResult> GetInlineAttachmentAsync(DialogContext dc, Object options)
        {
            var imagePath = Path.Combine(Environment.CurrentDirectory, @"Resources", "architecture-resize.png");
            var imageData = Convert.ToBase64String(File.ReadAllBytes(imagePath));

            // Store the base64 url in turn.contentUrl.
            dc.State.SetValue("turn.contentUrl", $"data:image/png;base64,{imageData}");

            return await dc.EndDialogAsync().ConfigureAwait(false);
        }

        // Creates an "Attachment" to be sent from the bot to the user from an uploaded file.
        private async Task<DialogTurnResult> UploadAttachmentAsync(DialogContext dc, Object options)
        {
            var imagePath = Path.Combine(Environment.CurrentDirectory, @"Resources", "architecture-resize.png");
            var connector = dc.Context.TurnState.Get<IConnectorClient>() as ConnectorClient;
            var attachments = new Attachments(connector);
            var activity = dc.Context.Activity;

            // Note: This api does not work on all channels.
            var response = await attachments.Client.Conversations.UploadAttachmentAsync(
                activity.Conversation.Id,
                new AttachmentData
                {
                    Name = @"Resources\architecture-resize.png",
                    OriginalBase64 = File.ReadAllBytes(imagePath),
                    Type = "image/png",
                }).ConfigureAwait(false);

            var attachmentUri = attachments.GetAttachmentUri(response.Id);
            dc.State.SetValue("turn.attachmentUri", attachmentUri);

            return await dc.EndDialogAsync().ConfigureAwait(false);
        }

        private static List<Dialog> WelcomeUserSteps()
        {
            return new List<Dialog>()
            {
                // Iterate through membersAdded list and greet user added to the conversation.
                new Foreach()
                {
                    ItemsProperty = "turn.activity.membersAdded",
                    Actions = new List<Dialog>()
                    {
                        new IfCondition()
                        {
                            Condition = "$foreach.value.name != turn.activity.recipient.name",
                            Actions = new List<Dialog>()
                            {
                                new SendActivity("${WelcomeMessage()}")
                            }
                        }
                    }
                }
            };
        }
    }
}
