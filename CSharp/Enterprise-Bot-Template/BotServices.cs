// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Microsoft.ApplicationInsights;
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.AI.QnA;

namespace EnterpriseBot
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
            QnAServices = new Dictionary<string, QnAMaker>();
        }

        public string AuthConnectionName { get; set; }

        public TelemetryClient TelemetryClient { get; set; }

        public LuisRecognizer DispatchRecognizer { get; set; }

        public Dictionary<string, LuisRecognizer> LuisServices { get; set; }

        public Dictionary<string, QnAMaker> QnAServices { get; set; }
    }
}
