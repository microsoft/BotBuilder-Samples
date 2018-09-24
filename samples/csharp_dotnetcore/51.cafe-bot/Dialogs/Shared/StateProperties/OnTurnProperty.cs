// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Newtonsoft.Json;
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

        public string Intent { get; set; }

        public List<EntityProperty> Entities { get; set; }

        public OnTurnProperty()
        {
            Intent = null;
            Entities = null;
        }

        public OnTurnProperty(string intent, List<EntityProperty> entities)
        {
            Intent = intent ?? throw new ArgumentNullException(nameof(intent));
            Entities = entities ?? throw new ArgumentNullException(nameof(entities));
        }

        public static OnTurnProperty FromLuisResults(RecognizerResult LUISResults)
        {
            var onTurnProperties = new OnTurnProperty();
            onTurnProperties.Intent = LUISResults.GetTopScoringIntent().intent;

            // Gather entity values if available. Uses a const list of LUIS entity names.
            foreach (var entity in luisEntities)
            {
                onTurnProperties.Entities.Add(new EntityProperty(entity, (string)LUISResults.Entities[entity]));
            }

            return onTurnProperties;
        }

        /// <summary>
        /// Static method to create an on turn property object from card input.
        /// </summary>
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
                }
            }

            return onTurnProperties;
        }
    }
}
