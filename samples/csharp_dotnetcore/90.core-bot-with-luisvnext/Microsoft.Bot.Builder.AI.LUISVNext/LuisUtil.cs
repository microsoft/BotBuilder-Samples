// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Linq;

[assembly: InternalsVisibleTo("Microsoft.Bot.Builder.AI.LuisVNext.Tests")]
namespace Microsoft.Bot.Builder.AI.LuisVNext
{
    // Utility functions used to extract and transform data from LuisVNext SDK
    internal static class LuisUtil
    {
        internal static string NormalizedIntent(string intent) => intent.Replace('.', '_').Replace(' ', '_');

        internal static IDictionary<string, IntentScore> GetIntents(JObject luisResult)
        {
            var result = new Dictionary<string, IntentScore>();
            var intents = luisResult["intents"];
            if (intents != null)
            {
                foreach (var intent in intents)
                {
                    result.Add(NormalizedIntent(intent["category"].Value<string>()), new IntentScore { Score = intent["confidenceScore"] == null ? 0.0 : intent["confidenceScore"].Value<double>() });
                }
            }

            return result;
        }

        internal static JObject ExtractEntitiesAndMetadata(JObject prediction)
        {
            var entities = prediction["entities"];
            var entityObject = new JObject();
            entityObject.Add("entities", entities);

            return entityObject;
        }

        internal static void AddProperties(JObject luis, RecognizerResult result)
        {
            var topIntent = luis["topIntent"];
            var projectType = luis["projectType"];

            if (topIntent != null)
            {
                result.Properties.Add("topIntent", topIntent);
            }

            if(projectType != null)
            {
                result.Properties.Add("projectType", projectType);
            }
        }
    }
}
