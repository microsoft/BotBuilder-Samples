using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;

namespace DynamicsOmnichannelBot.AdaptiveCards
{
    public class AdaptiveCards
    {
        public static Attachment CreateAdaptiveCardAttachment(string card)
        {
            string[] paths = { ".", "AdaptiveCards", card };
            var adaptiveCardJson = File.ReadAllText(Path.Combine(paths));

            var attachment = new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(adaptiveCardJson),
            };
            return attachment;
        }

        public static SuggestedActions CreateSuggestedAction(string[] actions)
        {
            var actionsList = new List<CardAction>();
            foreach (string action in actions)
            {
                actionsList.Add(new CardAction() { Title = action, Type = ActionTypes.ImBack, Value = action });
            }

            return new SuggestedActions() { Actions = actionsList };
        }
    }
}
