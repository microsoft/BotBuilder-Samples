// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license.

using AdaptiveCards;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Schema;
using System;
using System.IO;

namespace TeamsTasksBot.Helper
{
    /// <summary>
    ///  Helper class which posts to the saved channel every 20 seconds.
    /// </summary>
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
         //   var path = System.Web.Hosting.HostingEnvironment.MapPath(@"~\Resources\Cards\AdaptiveCard.json");

            var path = System.IO.Path.Combine(WebRootPath, "Resources","Cards","AdaptiveCard.json");
            System.Diagnostics.Debug.WriteLine(path);

            return File.ReadAllText(path);
        }

    }
}
