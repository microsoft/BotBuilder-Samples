// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// RichCardsBot prompts a user to select a Rich Card and then returns the card
    /// that matches the user's selection.
    /// </summary>
    /// <typeparam name="T">The main dialog</typeparam>
    public class RichCardsBot<T> : DialogBot<T> where T : Dialog
    {
        public RichCardsBot(ConversationState conversationState, UserState userState, T dialog, ILogger<DialogBot<T>> logger)
            : base(conversationState, userState, dialog, logger)
        {
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                // Greet anyone that was not the target (recipient) of this message.
                // To learn more about Adaptive Cards, see https://aka.ms/msbot-adaptivecards for more details.
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    var reply = MessageFactory.Text("Welcome to CardBot."
                        + " This bot will show you different types of Rich Cards."
                        + " Please type anything to get started.");

                    await turnContext.SendActivityAsync(reply, cancellationToken);
                }
            }
        }
    }
}
