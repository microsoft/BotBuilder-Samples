// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License

using Microsoft.ApplicationInsights;
using Microsoft.Bot.Builder.AI.QnA;
using System;
using System.Collections.Generic;


namespace AspNetCore_QnA_Bot
{
    /// <summary>
    /// Class represents bot configuration.
    /// </summary>
    [Serializable]
    public class BotServices
    {
        public BotServices()
        {
            QnAServices = new Dictionary<string, QnAMaker>();
        }
        public string AuthConnectionName { get; set; }

        public TelemetryClient TelemetryClient { get; set; }

        public Dictionary<string, QnAMaker> QnAServices { get; set; }
    }
}