using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AdaptiveCards;
using Microsoft.Bot.Schema;

namespace TeamsCardActions
{
    public static class AdaptiveCardHelper
    {
        public static string WebRootPath { get; set; }

        public static Attachment GetAdaptiveCard()
        {
            // Parse the JSON 
            AdaptiveCardParseResult result = AdaptiveCard.FromJson(GetAdaptiveCardJson());

            return new Attachment()
            {
                ContentType = AdaptiveCard.ContentType,
                Content = result.Card

            };
        }

        public static String GetAdaptiveCardJson()
        {
            var path = System.IO.Path.Combine(WebRootPath, "Cards", "AdaptiveCard.json");
            System.Diagnostics.Debug.WriteLine(path);

            return File.ReadAllText(path);
        }
    }
}
