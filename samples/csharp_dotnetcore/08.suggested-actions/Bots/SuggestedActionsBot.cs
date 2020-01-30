// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples
{
    // This bot will respond to the user's input with suggested actions.
    // Suggested actions enable your bot to present buttons that the user
    // can tap to provide input. 
    public class SuggestedActionsBot : ActivityHandler
    {
        public const string WelcomeText = @"This bot will introduce you to suggestedActions.
                                            Please answer the question:";

      
        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            // Send a welcome message to the user and tell them what actions they may perform to use this bot
            await SendWelcomeMessageAsync(turnContext, cancellationToken);
        }
        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {

            // Extract the text from the message activity the user sent.
            var text = turnContext.Activity.Text.ToLowerInvariant();

            // Take the input from the user and create the appropriate response.
            var responseText = ProcessInput(text);

            // Respond to the user.
            await turnContext.SendActivityAsync(responseText, cancellationToken: cancellationToken);

            await SendSuggestedActionsAsync(turnContext, cancellationToken);
        }
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

        // Creates and sends an activity with suggested actions to the user. When the user
        /// clicks one of the buttons the text value from the "CardAction" will be
        /// displayed in the channel just as if the user entered the text. There are multiple
        /// "ActionTypes" that may be used for different situations.
        private static async Task SendSuggestedActionsAsync(ITurnContext turnContext, CancellationToken cancellationToken)
        {
            var reply = MessageFactory.Text("What is your favorite color?");

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
