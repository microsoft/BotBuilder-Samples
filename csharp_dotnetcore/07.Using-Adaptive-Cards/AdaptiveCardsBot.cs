// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// This bot will respond to the user's input with an Adaptive Card.
    /// Adaptive Cards are a way for developers to exchange card content
    /// in a common and consistent way. A simple open card format enables
    /// an ecosystem of shared tooling, seamless integration between apps,
    /// and native cross-platform performance on any device.
    /// </summary>
    public class AdaptiveCardsBot : IBot
    {
        // This arrary contains the file location of our adaptive cards
        private readonly string[] _cards =
        {
            @".\Resources\FlightItineraryCard.json",
            @".\Resources\ImageGalleryCard.json",
            @".\Resources\LargeWeatherCard.json",
            @".\Resources\RestaurantCard.json",
            @".\Resources\SolitaireCard.json",
        };

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
                    Random r = new Random();
                    var cardAttachment = CreateAdaptiveCardAttachment(this._cards[r.Next(0, this._cards.Length - 1)]);
                    var reply = turnContext.Activity.CreateReply();
                    reply.Attachments = new List<Attachment>() { cardAttachment };
                    await turnContext.SendActivityAsync(reply, cancellationToken);
                    await turnContext.SendActivityAsync("Please enter any text to see another card.", cancellationToken: cancellationToken);
                    break;
                case ActivityTypes.ConversationUpdate:
                    // Send a welcome & help message to the user.
                    if (turnContext.Activity.MembersAdded.Any())
                    {
                        await SendWelcomeMessageAsync(turnContext, cancellationToken);
                    }

                    break;
            }
        }

        /// <summary>
        /// Greet new users as they are added to the conversation.
        /// </summary>
        /// <param name="turnContext">Provides the <see cref="ITurnContext"/> for the turn of the bot.</param>
        /// <param name="cancellationToken" >(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the operation result of the Turn operation.</returns>
        private static async Task SendWelcomeMessageAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in turnContext.Activity.MembersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(
                        $"Welcome to AdaptiveCardsBot {member.Name}." +
                        $" This bot will introduce you to AdaptiveCards." +
                        $" Type anything to see an AdaptiveCard.",
                        cancellationToken: cancellationToken);
                }
            }
        }

        /// <summary>
        /// Creates an <see cref="Attachment"/> that contains an <see cref="AdaptiveCard"/>.
        /// </summary>
        /// <param name="filePath">The path to the <see cref="AdaptiveCard"/> json file.</param>
        /// <returns>An <see cref="Attachment"/> that contains an adaptive card.</returns>
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
