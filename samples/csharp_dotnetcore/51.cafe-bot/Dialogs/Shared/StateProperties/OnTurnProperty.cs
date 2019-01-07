// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
<<<<<<< HEAD
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Newtonsoft.Json;
=======
using Microsoft.Bot.Builder;
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
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
<<<<<<< HEAD
                dynamic value = luisResults.Entities[entity];
                string strVal = null;
                if (value is JArray)
                {
                    // ConfirmList is nested arrays.
                    value = (from val in (JArray)value
                              select val).FirstOrDefault();
                }

                strVal = (string)value;

                if (strVal == null)
                {
                    // Don't add empty entities.
                    continue;
                }

                onTurnProperties.Entities.Add(new EntityProperty(entity, strVal));
=======
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
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
            }

            return onTurnProperties;
        }

        /// <summary>
        /// Static method to create an on turn property object from card input.
        /// </summary>
<<<<<<< HEAD
        /// <param name="cardValue">context.activity.value from a card interaction</param>
        /// <returns>OnTurnProperty.</returns>
        public static OnTurnProperty FromCardInput(Dictionary<string, string> cardValues)
        {
            // All cards used by this bot are adaptive cards with the card's 'data' property set to useful information.
            var onTurnProperties = new OnTurnProperty();
            foreach (KeyValuePair<string, string> entry in cardValues)
            {
                if (!string.IsNullOrWhiteSpace(entry.Key) && string.Compare(entry.Key.ToLower().Trim(), "intent") == 0)
                {
                    onTurnProperties.Intent = cardValues[entry.Key];
                }
                else
                {
                    onTurnProperties.Entities.Add(new EntityProperty(entry.Key, cardValues[entry.Key]));
=======
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
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
                }
            }

            return onTurnProperties;
        }
    }
}
