// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Linq;

[assembly: InternalsVisibleTo("Microsoft.Bot.Builder.AI.CLU.Tests")]
namespace Microsoft.Bot.Builder.AI.CLU
{
    // Utility functions used to extract and transform data from CLU
    internal static class CluUtil
    {
        internal static string NormalizedIntent(string intent) => intent.Replace('.', '_').Replace(' ', '_');

        internal static IDictionary<string, IntentScore> GetIntents(JObject cluResult)
        {
            var result = new Dictionary<string, IntentScore>();
            var intents = cluResult["intents"];
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

        internal static void AddProperties(JObject clu, RecognizerResult result)
        {
            var topIntent = clu["topIntent"];
            var projectType = clu["projectType"];

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
