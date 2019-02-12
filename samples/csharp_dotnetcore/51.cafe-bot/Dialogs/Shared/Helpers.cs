// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;

namespace Microsoft.BotBuilderSamples
{
    public static class Helpers
    {
        private static readonly int DefaultNumberOfSuggestions = 3;
        private static readonly string[] PromotionsList = { "Book a table", "Who are you?", "Sing a song" };

        public static string[] GenSuggestedQueries(List<string> dispatchLUISModel = null, int numberOfSuggestions = 0)
        {
            var suggestedQueries = new List<string> { "What can you do?" };
            if (PromotionsList.Count() != 0)
            {
                var rnd = new Random();
                var rndIdx = (int)Math.Floor(PromotionsList.Count() * rnd.NextDouble());
                suggestedQueries.Add(PromotionsList.ElementAt(rndIdx));
            }

            var possibleUtterances = dispatchLUISModel;
            if (numberOfSuggestions == 0)
            {
                numberOfSuggestions = DefaultNumberOfSuggestions;
            }

            while (--numberOfSuggestions > 0 && possibleUtterances != null)
            {
                var rnd = new Random();
                var rndIdx = (int)Math.Floor(possibleUtterances.Count * rnd.NextDouble());
                suggestedQueries.Add(possibleUtterances.ElementAt(rndIdx));
            }

            return suggestedQueries.ToArray();
        }

        // Create an attachment message response.
        public static Activity CreateResponse(Activity activity, Attachment attachment)
        {
            var response = activity.CreateReply();
            response.Attachments = new List<Attachment>() { attachment };
            return response;
        }

        // Load attachment from file
        public static Attachment CreateAdaptiveCardAttachment(string[] directories)
        {
            // combine path for cross platform support
            string fullPath = Path.Combine(directories);
            var adaptiveCard = File.ReadAllText(fullPath);
            return new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(adaptiveCard),
            };
        }
    }
}
