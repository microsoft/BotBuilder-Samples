// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using Microsoft.Bot.Schema;
using QnAPrompting.Models;

namespace QnAPrompting.Helpers
{
    public class CardHelper
    {
        /// <summary>
        /// Get Hero card
        /// </summary>
        /// <param name="cardTitle">Title of the card</param>
        /// <param name="prompts">List of suggested prompts</param>
        /// <returns>Message activity</returns>
        public static Activity GetHeroCard(string cardTitle, QnAPrompts[] prompts)
        {
            var chatActivity = Activity.CreateMessageActivity();
            var buttons = new List<CardAction>();

            var sortedPrompts = prompts.OrderBy(r => r.DisplayOrder);
            foreach (var prompt in sortedPrompts)
            {
                buttons.Add(
                    new CardAction()
                    {
                        Value = prompt.DisplayText,
                        Type =  ActionTypes.ImBack,
                        Title = prompt.DisplayText,
                    });
            }

            var plCard = new HeroCard()
            {
                Title = cardTitle,
                Subtitle = string.Empty,
                Buttons = buttons
            };

            var attachment = plCard.ToAttachment();

            chatActivity.Attachments.Add(attachment);

            return (Activity)chatActivity;
        }
    }
}
