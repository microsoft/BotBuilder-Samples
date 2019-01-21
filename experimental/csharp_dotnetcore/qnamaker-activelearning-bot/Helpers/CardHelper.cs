using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;

namespace QnAMakerActiveLearningBot.Helpers
{
    public class CardHelper
    {
        private const string DefaultNoMatchText = "None of the above.";
        private const string DefaultTitle = "Did you mean:";

        /// <summary>
        /// Get Hero card
        /// </summary>
        /// <param name="title">Title of the cards</param>
        /// <param name="noMatchText">No match text</param>
        /// <param name="suggestionsList">List of suggested questions</param>
        /// <returns></returns>
        public static IMessageActivity GetHeroCard(List<string> suggestionsList, string title = "", string noMatchText = "")
        {
            var cardTitle = string.IsNullOrEmpty(title) ? DefaultTitle : title;
            var cardNoMatchText = string.IsNullOrEmpty(noMatchText) ? DefaultNoMatchText : noMatchText;

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
