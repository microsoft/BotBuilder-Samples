// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Bot.Builder;
using Newtonsoft.Json.Linq;

namespace Microsoft.BotBuilderSamples
{
    public class OnTurnProperty
    {
        private static string[] luisEntities =
        {
            "confirmationList",
            "number",
            "datetime",
            "cafeLocation",
            "userName_patternAny",
            "userName",
        };

        public OnTurnProperty()
        {
            Intent = null;
            Entities = new List<EntityProperty>();
        }

        public OnTurnProperty(string intent, List<EntityProperty> entities)
        {
            Intent = intent ?? throw new ArgumentNullException(nameof(intent));
            Entities = entities ?? throw new ArgumentNullException(nameof(entities));
        }

        public string Intent { get; set; }

        public List<EntityProperty> Entities { get; set; }

        public static OnTurnProperty FromLuisResults(RecognizerResult luisResults)
        {
            var onTurnProperties = new OnTurnProperty();
            onTurnProperties.Intent = luisResults.GetTopScoringIntent().intent;

            // Gather entity values if available. Uses a const list of LUIS entity names.
            foreach (var entity in luisEntities)
            {
                var value = luisResults.Entities.SelectTokens(entity).FirstOrDefault();
                if (value == null)
                {
                    continue;
                }

                object property = null;
                var val = value.First();
                if (val.Type == JTokenType.Array)
                {
                    var arr = (JArray)val;
                    property = arr[0].ToString(); // Store first value
                }
                else if (val.Type == JTokenType.Object)
                {
                    var obj = (JObject)val;
                    if (obj["type"].ToString() == "datetime")
                    {
                        property = val;  // Store the JToken from LUIS (includes Timex)
                    }
                }
                else if (val.Type == JTokenType.Integer)
                {
                    var num = (JValue)val;
                    property = val.ToString();  // Store string for number of guests
                }

                onTurnProperties.Entities.Add(new EntityProperty(entity, property));
            }

            return onTurnProperties;
        }

        /// <summary>
        /// Static method to create an on turn property object from card input.
        /// </summary>
        /// <param name="cardValues">context.activity.value from a card interaction</param>
        /// <returns>OnTurnProperty.</returns>
        public static OnTurnProperty FromCardInput(JObject cardValues)
        {
            // All cards used by this bot are adaptive cards with the card's 'data' property set to useful information.
            var onTurnProperties = new OnTurnProperty();
            foreach (var val in cardValues)
            {
                string name = val.Key;
                JToken value = val.Value;
                if (!string.IsNullOrWhiteSpace(name) && string.Compare(name.ToLower().Trim(), "intent") == 0)
                {
                    onTurnProperties.Intent = value.ToString();
                }
                else
                {
                    onTurnProperties.Entities.Add(new EntityProperty(name, value.ToString()));
                }
            }

            return onTurnProperties;
        }
    }
}
