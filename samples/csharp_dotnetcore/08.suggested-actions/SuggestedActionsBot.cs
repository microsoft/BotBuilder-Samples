// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// This bot will respond to the user's input with suggested actions.
    /// Suggested actions enable your bot to present buttons that the user
    /// can tap to provide input. Suggested actions appear close to the composer
    /// and enhance user experience by enabling the user to answer a question or
    /// make a selection with a simple tap of a button, rather than having to
    /// type a response with a keyboard. Unlike buttons that appear within rich
    /// cards (which remain visible and accessible to the user even after being tapped),
    /// buttons that appear within the suggested actions pane will disappear after
    /// the user makes a selection. This prevents the user from tapping stale buttons
    /// within a conversation and simplifies bot development (since you will not need
    /// to account for that scenario).
    /// For each user interaction, an instance of this class is created and the OnTurnAsync method is called.
    /// This is a Transient lifetime service. Transient lifetime services are created
    /// each time they're requested. For each Activity received, a new instance of this
    /// class is created. Objects that are expensive to construct, or have a lifetime
    /// beyond the single turn, should be carefully managed.
    /// </summary>
    public class SuggestedActionsBot : IBot
    {
        public const string WelcomeText = @"This bot will introduce you to suggestedActions.
                                            Please answer the question:";

        /// <summary>
        /// Every conversation turn for our bot will call this method.
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

            // Handle Message activity type, which is the main activity type for shown within a conversational interface
            // Message activities may contain text, speech, interactive cards, and binary or unknown attachments.
            // See https://aka.ms/about-bot-activity-message to learn more about the message and other activity types
            if (turnContext.Activity.Type == ActivityTypes.Message)
            {
                // Extract the text from the message activity the user sent.
                // Make this lowercase not accounting for culture in this case
                // so that there are fewer string variations which you will
                // have to account for in your bot.
                var text = turnContext.Activity.Text.ToLowerInvariant();

                // Take the input from the user and create the appropriate response.
                var responseText = ProcessInput(text);

                // Respond to the user.
                await turnContext.SendActivityAsync(responseText, cancellationToken: cancellationToken);

                await SendSuggestedActionsAsync(turnContext, cancellationToken);
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
                await turnContext.SendActivityAsync($"{turnContext.Activity.Type} event detected", cancellationToken: cancellationToken);
            }
        }

        /// <summary>
        /// On a conversation update activity sent to the bot, the bot will
        /// send a message to the any new user(s) that were added.
        /// </summary>
        /// <param name="turnContext">Provides the <see cref="ITurnContext"/> for the turn of the bot.</param>
        /// <param name="cancellationToken" >(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
        private static async Task SendWelcomeMessageAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in turnContext.Activity.MembersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    await turnContext.SendActivityAsync(
                        $"Welcome to SuggestedActionsBot {member.Name}. {WelcomeText}",
                        cancellationToken: cancellationToken);
                    await SendSuggestedActionsAsync(turnContext, cancellationToken);
                }
            }
        }

        /// <summary>
        /// Given the text from the message activity the user sent, create the text for the response.
        /// </summary>
        /// <param name="text">The text that was input by the user.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
        private static string ProcessInput(string text)
        {
            const string colorText = "is the best color, I agree.";
            switch (text)
            {
                case "red":
                {
                    return $"Red {colorText}";
                }

                case "yellow":
                {
                    return $"Yellow {colorText}";
                }

                case "blue":
                {
                    return $"Blue {colorText}";
                }

                default:
                {
                    return "Please select a color from the suggested action choices";
                }
            }
        }

        /// <summary>
        /// Creates and sends an activity with suggested actions to the user. When the user
        /// clicks one of the buttons the text value from the <see cref="CardAction"/> will be
        /// displayed in the channel just as if the user entered the text. There are multiple
        /// <see cref="ActionTypes"/> that may be used for different situations.
        /// </summary>
        /// <param name="turnContext">Provides the <see cref="ITurnContext"/> for the turn of the bot.</param>
        /// <param name="cancellationToken" >(Optional) A <see cref="CancellationToken"/> that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> that represents the work queued to execute.</returns>
        private static async Task SendSuggestedActionsAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var reply = turnContext.Activity.CreateReply("What is your favorite color?");
            reply.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                {
                    new CardAction() { Title = "Red", Type = ActionTypes.ImBack, Value = "Red" },
                    new CardAction() { Title = "Yellow", Type = ActionTypes.ImBack, Value = "Yellow" },
                    new CardAction() { Title = "Blue", Type = ActionTypes.ImBack, Value = "Blue" },
                },
            };
            await turnContext.SendActivityAsync(reply, cancellationToken);
        }
    }
}
