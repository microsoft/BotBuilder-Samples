// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Microsoft.ApplicationInsights;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.AI.QnA;

namespace MessageRoutingBot
{
    /// <summary>
    /// Class represents bot configuration.
    /// </summary>
    [Serializable]
    public class BotServices
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BotServices"/> class.
        /// </summary>
        public BotServices()
        {
            LuisServices = new Dictionary<string, LuisRecognizer>();
        }

        /// <summary>
        /// Gets or sets the map of LUIS services.
        /// </summary>
        /// <value>
        /// The <see cref="LuisRecognizer"> associated with the provided key.
        /// </value>.
        public Dictionary<string, LuisRecognizer> LuisServices { get; set; }
    }
}
