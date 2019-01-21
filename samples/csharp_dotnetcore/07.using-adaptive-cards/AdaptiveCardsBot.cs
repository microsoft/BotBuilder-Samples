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
using Remotion.Linq.Parsing.Structure.IntermediateModel;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// This bot will respond to the user's input with an Adaptive Card.
    /// Adaptive Cards are a way for developers to exchange card content
    /// in a common and consistent way. A simple open card format enables
    /// an ecosystem of shared tooling, seamless integration between apps,
    /// and native cross-platform performance on any device.
    /// For each user interaction, an instance of this class is created and the OnTurnAsync method is called.
    /// This is a Transient lifetime service. Transient lifetime services are created
    /// each time they're requested. For each Activity received, a new instance of this
    /// class is created. Objects that are expensive to construct, or have a lifetime
    /// beyond the single turn, should be carefully managed.
    /// </summary>
    public class AdaptiveCardsBot : IBot
    {
        private const string WelcomeText = @"This bot will introduce you to AdaptiveCards.
                                            Type anything to see an AdaptiveCard.";

        // This array contains the file location of our adaptive cards
        private readonly string[] _cards =
        {
            Path.Combine(".", "Resources", "FlightItineraryCard.json"),
            Path.Combine(".", "Resources", "ImageGalleryCard.json"),
            Path.Combine(".", "Resources", "LargeWeatherCard.json"),
            Path.Combine(".", "Resources", "RestaurantCard.json"),
            Path.Combine(".", "Resources", "SolitaireCard.json"),
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

            // Handle Message activity type, which is the main activity type for shown within a conversational interface
            // Message activities may contain text, speech, interactive cards, and binary or unknown attachments.
            // see https://aka.ms/about-bot-activity-message to learn more about the message and other activity types
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                Random r = new Random();
                var cardAttachment = CreateAdaptiveCardAttachment(this._cards[r.Next(this._cards.Length)]);
                var reply = turnContext.Activity.CreateReply();
                reply.Attachments = new List<Attachment>() { cardAttachment };
                await turnContext.SendActivityAsync(reply, cancellationToken);
                await turnContext.SendActivityAsync("Please enter any text to see another card.", cancellationToken: cancellationToken);
            }
            else if (turnContext.Activity.Type == ActivityTypes.ConversationUpdate)
            {
                if (turnContext.Activity.MembersAdded != null)
                {
                    await SendWelcomeMessageAsync(turnContext, cancellationToken);
                }
            }
            else
            {
                await turnContext.SendActivityAsync($"{turnContext.Activity.Type} activity detected", cancellationToken: cancellationToken);
            }
        }

        /// <summary>
        /// Greet new users as they are added to the conversation.
        /// </summary>
        /// <param name="turnContext">A <see cref="ITurnContext"/> containing all the data needed
        /// for processing this conversation turn. </param>
        /// <param name="cancellationToken">(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
        /// <seealso cref="BotStateSet"/>
        /// <seealso cref="ConversationState"/>
        /// <seealso cref="IMiddleware"/>
        private static async Task SendWelcomeMessageAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in turnContext.Activity.MembersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(
                        $"Welcome to AdaptiveCardsBot {member.Name}. {WelcomeText}",
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
