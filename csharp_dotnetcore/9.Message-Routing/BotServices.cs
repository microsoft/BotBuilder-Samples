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
        public BotServices()
        {
            LuisServices = new Dictionary<string, LuisRecognizer>();
        }
        
        public Dictionary<string, LuisRecognizer> LuisServices { get; set; }
    }
}
