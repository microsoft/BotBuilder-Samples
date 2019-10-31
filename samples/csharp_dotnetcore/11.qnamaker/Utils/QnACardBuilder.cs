// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples.Utils
{
    /// <summary>
    /// Message activity card builder for QnAMaker dialogs.
    /// </summary>
    public static class QnACardBuilder
    {
        /// <summary>
        /// Get active learning suggestions card.
        /// </summary>
        /// <param name="suggestionsList">List of suggested questions.</param>
        /// <param name="cardTitle">Title of the cards.</param>
        /// <param name="cardNoMatchText">No match text.</param>
        /// <returns>IMessageActivity.</returns>
        public static IMessageActivity GetSuggestionsCard(List<string> suggestionsList, string cardTitle, string cardNoMatchText)
        {
            if (suggestionsList == null)
            {
                throw new ArgumentNullException(nameof(suggestionsList));
            }

            if (cardTitle == null) 
            {
                throw new ArgumentNullException(nameof(cardTitle));
            }

            if (cardNoMatchText == null)
            {
                throw new ArgumentNullException(nameof(cardNoMatchText));
            }

            var chatActivity = Activity.CreateMessageActivity();
            var buttonList = new List<CardAction>();

            // Add all suggestions
            foreach (var suggestion in suggestionsList)
            {
                buttonList.Add(
                    new CardAction()
                    {
                        Value = suggestion,
                        Type = "imBack",
                        Title = suggestion,
                    });
            }

            // Add No match text
            buttonList.Add(
                new CardAction()
                {
                    Value = cardNoMatchText,
                    Type = "imBack",
                    Title = cardNoMatchText
                });

            var plCard = new HeroCard()
            {
                Text = cardTitle,
                Subtitle = string.Empty,
                Buttons = buttonList
            };

            // Create the attachment.
            var attachment = plCard.ToAttachment();

            chatActivity.Attachments.Add(attachment);

            return chatActivity;
        }

        /// <summary>
        /// Get multi-turn prompts card.
        /// </summary>
        /// <param name="result">Result to be dispalyed as prompts.</param>
        /// <returns>IMessageActivity.</returns>
        public static IMessageActivity GetQnAPromptsCard(QueryResult result)
        {
            if (result == null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            var chatActivity = Activity.CreateMessageActivity();
            var buttonList = new List<CardAction>();

            // Add all prompt
            foreach (var prompt in result.Context.Prompts)
            {
                buttonList.Add(
                    new CardAction()
                    {
                        Value = prompt.DisplayText,
                        Type = "imBack",
                        Title = prompt.DisplayText,
                    });
            }

            var plCard = new HeroCard()
            {
                Text = result.Answer,
                Subtitle = string.Empty,
                Buttons = buttonList
            };

            // Create the attachment.
            var attachment = plCard.ToAttachment();

            chatActivity.Attachments.Add(attachment);

            return chatActivity;
        }
    }
}
