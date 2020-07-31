// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

using Newtonsoft.Json;

namespace Microsoft.BotBuilderSamples
{
    // This bot will respond to the user's input with an Adaptive Card.
    // Adaptive Cards are a way for developers to exchange card content
    // in a common and consistent way. A simple open card format enables
    // an ecosystem of shared tooling, seamless integration between apps,
    // and native cross-platform performance on any device.
    // For each user interaction, an instance of this class is created and the OnTurnAsync method is called.
    // This is a Transient lifetime service. Transient lifetime services are created
    // each time they're requested. For each Activity received, a new instance of this
    // class is created. Objects that are expensive to construct, or have a lifetime
    // beyond the single turn, should be carefully managed.

    public class AdaptiveCardsBot : ActivityHandler
    {
        // This array contains the file location of our adaptive cards
        private readonly string[] _cards =
        {
            Path.Combine(".", "Resources", "FlightItineraryCard.json"),
            Path.Combine(".", "Resources", "FlightUpdateCard.json"),
            Path.Combine(".", "Resources", "ImageGalleryCard.json"),
            Path.Combine(".", "Resources", "LargeWeatherCard.json"),
            Path.Combine(".", "Resources", "RestaurantCard.json"),
            Path.Combine(".", "Resources", "SolitaireCard.json"),
            Path.Combine(".", "Resources", "ProductVideo.json"),
            Path.Combine(".", "Resources", "VideoAndAudio.json")
        };

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            await SendWelcomeMessageAsync(turnContext, cancellationToken);
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var attachments = new List<Attachment>();
            if (turnContext.Activity.Text.ToLower() == "carousel")
            {
                foreach (var card in _cards)
                {
                    var cardAttachment = CreateAdaptiveCardAttachment(card);
                    attachments.Add(cardAttachment);
                }
            }
            else
            {
                var cardAttachment = CreateAdaptiveCardAttachment(_cards[int.Parse(turnContext.Activity.Text)]);
                attachments.Add(cardAttachment);
            }

            var reply = MessageFactory.Attachment(attachments);
            if (turnContext.Activity.Text == "carousel")
            {
                reply.AttachmentLayout = "carousel";
            }

            await turnContext.SendActivityAsync(reply, cancellationToken);
            await turnContext.SendActivityAsync(MessageFactory.Text($"Please enter 0-{_cards.Length - 1} to see another card."), cancellationToken);
        }

        private async Task SendWelcomeMessageAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in turnContext.Activity.MembersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(
                        $"Welcome to Adaptive Cards Bot {member.Name}. \r\n" +
                        $"Please input 0-{_cards.Length - 1} to see different cards",
                        cancellationToken: cancellationToken);
                }
            }
        }

        private static Attachment CreateAdaptiveCardAttachment(string filePath)
        {
            var adaptiveCardJson = File.ReadAllText(filePath);
            var adaptiveCardAttachment = new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(adaptiveCardJson),
            };
            return adaptiveCardAttachment;
        }
    }
}






