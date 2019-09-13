using AdaptiveCards;
using Microsoft.Bot.Schema;
using System;
using System.IO;

namespace TaskModule
{
    public static class AdaptiveCardHelper
    {
        public static Attachment GetAdaptiveCard(string rootPath)
        {
            // Parse the JSON 
            AdaptiveCardParseResult result = AdaptiveCard.FromJson(GetAdaptiveCardJson(rootPath));

            return new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = result.Card

            };
        }

        public static String GetAdaptiveCardJson(string rootPath)
        {
            var path = System.IO.Path.Combine(rootPath, "Resources", "AdaptiveCard.json");
            System.Diagnostics.Debug.WriteLine(path);

            return File.ReadAllText(path);
        }
    }
}
