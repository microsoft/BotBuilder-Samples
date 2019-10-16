// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Microsoft.Bot.Schema.Teams;
using Microsoft.Bot.Builder.Teams;
using Newtonsoft.Json.Linq;

namespace Microsoft.BotBuilderSamples.Bots
{
    /*
     * This one already exists in JS.
     */
    public class SearchBasedMessagingExtensionBot : TeamsActivityHandler
    {
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            await turnContext.SendActivityAsync(MessageFactory.Text($"Echo: {turnContext.Activity.Text}"), cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(MessageFactory.Text($"Welcome to Echo Bot."), cancellationToken);
                }
            }
        }

        protected override async Task<MessagingExtensionResponse> OnTeamsMessagingExtensionQueryAsync(ITurnContext<IInvokeActivity> turnContext, MessagingExtensionQuery query, CancellationToken cancellationToken)
        {
            var searchQuery = query.Parameters[0].Value as string;
            var messagingExtensionResponse = new MessagingExtensionResponse();

            messagingExtensionResponse.ComposeExtension = CreateMessagingExtensionResult(new List<MessagingExtensionAttachment>
            {
                CreateSearchResultAttachment(searchQuery),
                CreateDummySearchResultAttachment(),
                CreateSelectItemsResultAttachment(searchQuery)
            });

            return messagingExtensionResponse;
        }

        protected override async Task<MessagingExtensionResponse> OnTeamsMessagingExtensionSelectItemAsync(ITurnContext<IInvokeActivity> turnContext, JObject query, CancellationToken cancellationToken)
        {
            var searchQuery = query.ToObject<SearchQuery>();
            var bfLogo = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQtB3AwMUeNoq4gUBGe6Ocj8kyh3bXa9ZbV7u1fVKQoyKFHdkqU";
            var card = new HeroCard
            {
                Title = $"You selected a search result!",
                Text = $"You searched for \"{searchQuery.Query}\"",
                Images = new List<CardImage>
                    {
                        new CardImage { Url = bfLogo }
                    }
            };

            var attachment = new MessagingExtensionAttachment
            {
                ContentType = HeroCard.ContentType,
                Content = card
            };

            var messagingExtensionResponse = new MessagingExtensionResponse();
            messagingExtensionResponse.ComposeExtension = CreateMessagingExtensionResult(new List<MessagingExtensionAttachment> { attachment });
            return messagingExtensionResponse;
        }

        private MessagingExtensionResult CreateMessagingExtensionResult(List<MessagingExtensionAttachment> attachments)
        {
            return new MessagingExtensionResult
            {
                Type = "result",
                AttachmentLayout = "list",
                Attachments = attachments
            };
        }

        private MessagingExtensionAttachment CreateSearchResultAttachment(string searchQuery)
        {
            var cardText = $"You said \"{searchQuery}\"";
            var bfLogo = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQtB3AwMUeNoq4gUBGe6Ocj8kyh3bXa9ZbV7u1fVKQoyKFHdkqU";

            var button = new CardAction
            {
                Type = "openUrl",
                Title = "Click for more Information",
                Value = "https://docs.microsoft.com/en-us/microsoftteams/platform/concepts/bots/bots-overview"
            };

            var images = new List<CardImage>();
            images.Add(new CardImage(bfLogo));
            var buttons = new List<CardAction>();
            buttons.Add(button);

            var heroCard = new HeroCard("You searched for:", text: cardText, images: images, buttons: buttons);
            var preview = new HeroCard("You searched for:", text: cardText, images: images);

            return new MessagingExtensionAttachment
            {
                ContentType = HeroCard.ContentType,
                Content = heroCard,
                Preview = preview.ToAttachment()
            };
        }

        private MessagingExtensionAttachment CreateDummySearchResultAttachment()
        {
            var cardText = "https://docs.microsoft.com/en-us/microsoftteams/platform/concepts/bots/bots-overview";
            var bfLogo = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQtB3AwMUeNoq4gUBGe6Ocj8kyh3bXa9ZbV7u1fVKQoyKFHdkqU";

            var button = new CardAction
            {
                Type = "openUrl",
                Title = "Click for more Information",
                Value = "https://docs.microsoft.com/en-us/microsoftteams/platform/concepts/bots/bots-overview"
            };

            var images = new List<CardImage>();
            images.Add(new CardImage(bfLogo));
            var buttons = new List<CardAction>();
            buttons.Add(button);

            var heroCard = new HeroCard("Learn more about Teams:", text: cardText, images: images, buttons: buttons);
            var preview = new HeroCard("Learn more about Teams:", text: cardText, images: images);

            return new MessagingExtensionAttachment
            {
                ContentType = HeroCard.ContentType,
                Content = heroCard,
                Preview = preview.ToAttachment()
            };
        }

        private MessagingExtensionAttachment CreateSelectItemsResultAttachment(string searchQuery)
        {
            var bfLogo = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcQtB3AwMUeNoq4gUBGe6Ocj8kyh3bXa9ZbV7u1fVKQoyKFHdkqU";
            var cardText = $"You said \"{searchQuery}\"";

            var button = new CardAction
            {
                Type = "openUrl",
                Title = "Click for more Information",
                Value = "https://docs.microsoft.com/en-us/microsoftteams/platform/concepts/bots/bots-overview"
            };

            var images = new List<CardImage>();
            images.Add(new CardImage(bfLogo));
            var buttons = new List<CardAction>();
            buttons.Add(button);
            var selectItemTap = new CardAction
            {
                Type = "invoke",
                Value = new SearchQuery { Query = searchQuery }
            };

            var heroCard = new HeroCard(cardText, text: cardText, images: images);
            var preview = new HeroCard(cardText, text: cardText, images: images, tap: selectItemTap);

            return new MessagingExtensionAttachment
            {
                ContentType = HeroCard.ContentType,
                Content = heroCard,
                Preview = preview.ToAttachment()
            };
        }
    }
}
