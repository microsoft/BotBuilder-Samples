// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using Microsoft.ApplicationInsights;
using Microsoft.Bot.Builder.AI.QnA;
using Microsoft.Extensions.Logging;

namespace AspNetWebApi_QnA_Bot
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
            ILoggerFactory logger = new LoggerFactory();
            var log = logger.CreateLogger("QnaBot");
            log.LogDebug("Singleton was started.");
        }

        public string AuthConnectionName { get; set; }

        public TelemetryClient TelemetryClient { get; set; }

        public Dictionary<string, QnAMaker> QnAServices { get; set; }
    }
}