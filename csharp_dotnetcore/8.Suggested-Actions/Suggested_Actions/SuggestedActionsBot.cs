// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace Suggested_Actions
{
    /// <summary>
    /// This bot will respond to the user's input suggested actions.
    /// </summary>
    public class SuggestedActionsBot : IBot
    {
        /// <summary>
        /// This controls what happens when an activity gets sent to the bot.
        /// </summary>
        /// <param name="turnContext">Provides the context for the turn of the bot.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A task.</returns>
        public async Task OnTurnAsync(ITurnContext turnContext, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (turnContext == null)
            {
                throw new ArgumentNullException(nameof(turnContext));
            }

            switch (turnContext.Activity.Type)
            {
                case ActivityTypes.Message:
                    var text = turnContext.Activity.Text.ToLowerInvariant();
                    switch (text)
                    {
                        case "red":
                        {
                            await turnContext.SendActivityAsync(
                                "Red is the best color, I agree.",
                                cancellationToken: cancellationToken);
                            break;
                        }
                        case "yellow":
                        {
                            await turnContext.SendActivityAsync(
                                "Yellow is the best color, I agree.",
                                cancellationToken: cancellationToken);
                            break;
                        }

                        case "blue":
                        {
                            await turnContext.SendActivityAsync(
                                "Blue is the best color, I agree.",
                                cancellationToken: cancellationToken);
                            break;
                        }

                        default:
                        {
                            await turnContext.SendActivityAsync(
                                $"Please select a color from the suggested actions",
                                cancellationToken: cancellationToken);
                            break;
                        }
                    }

                    await SendSuggestedActionsAsync(turnContext, cancellationToken);
                    break;
                case ActivityTypes.ConversationUpdate:

                    // Send a welcome message to the user and tell them what actions they need to perform to use this bot
                    if (turnContext.Activity.MembersAdded.Any())
                    {
                        {
                            foreach (var member in turnContext.Activity.MembersAdded)
                            {
                                var newUserName = member.Name;
                                if (member.Id != turnContext.Activity.Recipient.Id)
                                {
                                    await turnContext.SendActivityAsync(
                                        $"Welcome to SuggestedActionsBot {member.Name}." +
                                        $" This bot will introduce you to suggestedActions." +
                                        $"  Please answer the question:",
                                        cancellationToken: cancellationToken);
                                    await SendSuggestedActionsAsync(turnContext, cancellationToken);
                                }
                            }
                        }
                    }

                    break;
            }
        }

        /// <summary>
        /// Creates and sends an activity with suggested actions to the user.
        /// </summary>
        /// <param name="turnContext">Provides the context for the turn of the bot.</param>
        /// <param name="cancellationToken">A cancellation token that can be used by other objects
        /// or threads to receive notice of cancellation.</param>
        /// <returns>A <see cref="Task"/> representing the operation result of the operation.</returns>
        private static async Task SendSuggestedActionsAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var reply = turnContext.Activity.CreateReply("What is your favorite color");
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
