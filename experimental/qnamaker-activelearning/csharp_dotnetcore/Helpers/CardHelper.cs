using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;

namespace QnAMakerActiveLearningBot.Helpers
{
    public class CardHelper
    {
        /// <summary>
        /// Get Hero card
        /// </summary>
        /// <param name="suggestionsList">List of suggested questions</param>
        /// <param name="cardTitle">Title of the cards</param>
        /// <param name="cardNoMatchText">No match text</param>
        /// <returns></returns>
        public static IMessageActivity GetHeroCard(List<string> suggestionsList, string cardTitle = "Did you mean:", string cardNoMatchText = "None of the above.")
        {
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
                Title = cardTitle,
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
