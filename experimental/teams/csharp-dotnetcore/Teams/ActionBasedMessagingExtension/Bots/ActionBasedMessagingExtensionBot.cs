// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Teams;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Newtonsoft.Json.Linq;

namespace Microsoft.BotBuilderSamples.Bots
{
    /*
     * After installing this bot you can click on the 3 dots to pull up the extension menu. You should be able to click on the search extension
     * and the bot will start. Interact with the cards to execrcise the flows. 
     */
    public class ActionBasedMessagingExtensionBot : TeamsActivityHandler
    {
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text($"echo: {turnContext.Activity.Text}"), cancellationToken);
        }

        protected override async Task<MessagingExtensionActionResponse> OnTeamsMessagingExtensionSubmitActionAsync(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionAction action, CancellationToken cancellationToken)
        {
            switch (action.CommandId)
            {
                // These commandIds are defined in the Teams App Manifest.
                case "createCard":
                    return HandleCreateCardCommands(turnContext, action);

                case "shareMessage":
                    return HandleShareMessageCommand(turnContext, action);
                default:
                    return new MessagingExtensionActionResponse();
            }
        }

        private MessagingExtensionActionResponse HandleCreateCardCommands(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionAction action)
        {
            var actionData = action.Data;
            var createCardData = ((JObject)actionData).ToObject<CreateCardData>();
            var response = new MessagingExtensionActionResponse
            {
                ComposeExtension = new MessagingExtensionResult
                {
                    AttachmentLayout = "list",
                    Type = "result",
                },
            };

            var card = new HeroCard
            {
                Title = createCardData.Title,
                Subtitle = createCardData.Subtitle,
                Text = createCardData.Text,
            };

            var attachments = new List<MessagingExtensionAttachment>();
            attachments.Add(new MessagingExtensionAttachment
            {
                Content = card,
                ContentType = HeroCard.ContentType,
                Preview = card.ToAttachment(),
            });

            response.ComposeExtension.Attachments = attachments;
            return response;
        }

        private MessagingExtensionActionResponse HandleShareMessageCommand(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionAction action)
        {
            var heroCard = new HeroCard
            {
                Title = $"{action.MessagePayload.From.User.DisplayName} sent this message:",
                Text = action.MessagePayload.Body.Content,
            };
            var actionData = action.Data;
            var includeImageData = ((JObject)actionData).ToObject<IncludeImageData>();
            if (includeImageData.IncludeImage)
            {
                heroCard.Images = new List<CardImage>
                {
                    new CardImage
                    {
                        Url = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQtB3AwMUeNoq4gUBGe6Ocj8kyh3bXa9ZbV7u1fVKQoyKFHdkqU",
                    },
                };
            }

            var response = new MessagingExtensionActionResponse();
            response.ComposeExtension = new MessagingExtensionResult
            {
                Type = "result",
                AttachmentLayout = "list",
                Attachments = new List<MessagingExtensionAttachment>
                {
                    new MessagingExtensionAttachment
                    {
                        Content = heroCard,
                        ContentType = HeroCard.ContentType,
                        Preview = heroCard.ToAttachment(),
                    },
                },
            };

            return response;
        }

        private class CreateCardData
        {
            public string Id { get; set; }

            public string Title { get; set; }

            public string Subtitle { get; set; }

            public string Text { get; set; }
        }

        private class IncludeImageData
        {
            public bool IncludeImage { get; set; }
        }
    }
}
