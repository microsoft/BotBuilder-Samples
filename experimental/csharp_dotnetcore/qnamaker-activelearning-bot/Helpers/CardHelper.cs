using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Bot.Schema;

namespace QnAMakerActiveLearningBot.Helpers
{
    public class CardHelper
    {
        private const string NO_MATCH = "None of the above";
        private const string TITLE = "Did you mean:";

        /// <summary>
        /// Get Hero card
        /// </summary>
        /// <param name="title">Title of the cards</param>
        /// <param name="noMatch">No match text</param>
        /// <param name="suggestionsList">List of suggested questions</param>
        /// <returns></returns>
        public static IMessageActivity GetHeroCard(string title, string noMatch, List<string> suggestionsList)
        {
            var chatActivity = Activity.CreateMessageActivity();

            var buttonList = new List<CardAction>();
            var plButton = new CardAction();

            foreach (var suggestion in suggestionsList)
            {
                plButton = new CardAction()
                {
                    Value = suggestion,
                    Type = "imBack",
                    Title = suggestion,
                };
                buttonList.Add(plButton);
            }

            plButton = new CardAction()
            {
                Value = noMatch,
                Type = "imBack",
                Title = noMatch
            };
            buttonList.Add(plButton);

            var plCard = new HeroCard()
            {
                Title = title,
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
