// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Microsoft.BotBuilderSamples.Bots
{
    public class TeamsMessagingExtensionsActionPreviewBot : TeamsActivityHandler
    {
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            if (turnContext.Activity.Value != null)
            {
                // This was a message from the card.
                var obj = (JObject)turnContext.Activity.Value;
                var answer = obj["Answer"]?.ToString();
                var choices = obj["Choices"]?.ToString();
                await turnContext.SendActivityAsync(MessageFactory.Text($"{turnContext.Activity.From.Name} answered '{answer}' and chose '{choices}'."), cancellationToken);
            }
            else
            {
                // This is a regular text message.
                await turnContext.SendActivityAsync(MessageFactory.Text($"Hello from the TeamsMessagingExtensionsActionPreviewBot."), cancellationToken);
            }
        }

        protected override Task<MessagingExtensionActionResponse> OnTeamsMessagingExtensionFetchTaskAsync(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionAction action, CancellationToken cancellationToken)
        {
            var adaptiveCardEditor = AdaptiveCardHelper.CreateAdaptiveCardEditor();

            return Task.FromResult(new MessagingExtensionActionResponse
            {
                Task = new TaskModuleContinueResponse
                {
                    Value = new TaskModuleTaskInfo
                    {
                        Card = new Attachment
                        {
                            Content = adaptiveCardEditor,
                            ContentType = AdaptiveCard.ContentType,
                        },
                        Height = 450,
                        Width = 500,
                        Title = "Task Module Fetch Example",
                    },
                },
            });
        }

        protected override Task<MessagingExtensionActionResponse> OnTeamsMessagingExtensionSubmitActionAsync(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionAction action, CancellationToken cancellationToken)
        {
            var exampleData = JsonConvert.DeserializeObject<ExampleData>(action.Data.ToString());

            var adaptiveCard = AdaptiveCardHelper.CreateAdaptiveCard(exampleData);

            // a number of reasonable options here...

            // (1) send a message on a new conversation and return null (only works in group chats and teams)

            // THIS WILL WORK IF THE BOT IS INSTALLED. (GetMembers() will NOT throw if the bot is installed.)

            //var message = MessageFactory.Attachment(new Attachment { ContentType = AdaptiveCard.ContentType, Content = adaptiveCard });
            //var channelId = turnContext.Activity.TeamsGetChannelId();
            //await turnContext.TeamsCreateConversationAsync(channelId, message, cancellationToken);
            //return null;

            // (2) drop the content into the compose window ready for the user to send

            //return new MessagingExtensionActionResponse
            //{
            //    ComposeExtension = new MessagingExtensionResult
            //    {
            //        Type = "result",
            //        AttachmentLayout = "list",
            //        Attachments = new List<MessagingExtensionAttachment>
            //        {
            //            new MessagingExtensionAttachment
            //            {
            //                Content = adaptiveCard,
            //                ContentType = AdaptiveCard.ContentType,
            //            },
            //        },
            //    },
            //};

            // (3) start a preview flow

            return Task.FromResult(new MessagingExtensionActionResponse
            {
                ComposeExtension = new MessagingExtensionResult
                {
                    Type = "botMessagePreview",
                    ActivityPreview = MessageFactory.Attachment(new Attachment
                    {
                        Content = adaptiveCard,
                        ContentType = AdaptiveCard.ContentType,
                    }) as Activity,
                },
            });
        }

        protected override Task<MessagingExtensionActionResponse> OnTeamsMessagingExtensionBotMessagePreviewEditAsync(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionAction action, CancellationToken cancellationToken)
        {
            // The data has been returned to the bot in the action structure.
            var activityPreview = action.BotActivityPreview[0];
            var attachmentContent = activityPreview.Attachments[0].Content;
            var previewedCard = JsonConvert.DeserializeObject<AdaptiveCard>(attachmentContent.ToString(), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            var exampleData = AdaptiveCardHelper.CreateExampleData(previewedCard);

            // This is a preview edit call and so this time we want to re-create the adaptive card editor.
            var adaptiveCardEditor = AdaptiveCardHelper.CreateAdaptiveCardEditor(exampleData);

            return Task.FromResult(new MessagingExtensionActionResponse
            {
                Task = new TaskModuleContinueResponse
                {
                    Value = new TaskModuleTaskInfo
                    {
                        Card = new Attachment
                        {
                            Content = adaptiveCardEditor,
                            ContentType = AdaptiveCard.ContentType,
                        },
                        Height = 450,
                        Width = 500,
                        Title = "Task Module Fetch Example",
                    },
                },
            });
        }

        protected override async Task<MessagingExtensionActionResponse> OnTeamsMessagingExtensionBotMessagePreviewSendAsync(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionAction action, CancellationToken cancellationToken)
        {
            // The data has been returned to the bot in the action structure.
            var activityPreview = action.BotActivityPreview[0];
            var attachmentContent = activityPreview.Attachments[0].Content;
            var previewedCard = JsonConvert.DeserializeObject<AdaptiveCard>(attachmentContent.ToString(), new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
            previewedCard.Version = "1.0";
            var exampleData = AdaptiveCardHelper.CreateExampleData(previewedCard);

            // This is a send so we are done and we will create the adaptive card editor.
            var adaptiveCard = AdaptiveCardHelper.CreateAdaptiveCard(exampleData);

            var message = MessageFactory.Attachment(new Attachment { ContentType = AdaptiveCard.ContentType, Content = adaptiveCard });

            //User Attribution for Bot messages
            message.ChannelData = new
            {
                OnBehalfOf = new[]
               {
                    new
                       {
                         ItemId = 0,
                         MentionType = "person",
                         Mri = turnContext.Activity.From.Id,
                         DisplayName = turnContext.Activity.From.Name
                    }
                }
            };

            // THIS WILL WORK IF THE BOT IS INSTALLED. (SendActivityAsync will throw if the bot is not installed.)
            await turnContext.SendActivityAsync(message, cancellationToken);

            return null;
        }

        protected override async Task OnTeamsMessagingExtensionCardButtonClickedAsync(ITurnContext<IInvokeActivity> turnContext, JObject obj, CancellationToken cancellationToken)
        {
            // If the adaptive card was added to the compose window (by either the OnTeamsMessagingExtensionSubmitActionAsync or
            // OnTeamsMessagingExtensionBotMessagePreviewSendAsync handler's return values) the submit values will come in here.
            var reply = MessageFactory.Text("OnTeamsMessagingExtensionCardButtonClickedAsync Value: " + JsonConvert.SerializeObject(turnContext.Activity.Value));
            await turnContext.SendActivityAsync(reply, cancellationToken);
        }
    }
}
