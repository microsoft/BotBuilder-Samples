// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using Microsoft.ApplicationInsights;
using Microsoft.Bot.Builder.AI.QnA;

namespace AspNetWebApi_QnA_Bot
{
    /// <summary>
    /// Class represents the bot's references to external services.
    ///
    /// For example, Application Insights and QnaMaker services
    /// are kept here (singletons).  These external services are configured
    /// using the BotConfigure class (based on the contents of your ".bot" file).
    /// </summary>
    [Serializable]
    public class BotServices
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BotServices"/> class.
        /// </summary>
        public BotServices()
        {
            // Given there can be multiple Qna services used in a single bot,
            // Qna is represented as a Dictionary.  This is also modeled in the
            // ".bot" file since the elements are named (string).
            QnAServices = new Dictionary<string, QnAMaker>();
        }

        /// <summary>
        /// Gets or sets the Application Insights Telemetry client.
        /// Use this to log new custom events/metrics/traces/etc into your
        /// Application Insights service for later analysis.
        /// </summary>
        /// <value>
        /// The Application Insights Telemetry client instance created based on configuration in the .bot file.
        /// </value>
        public TelemetryClient TelemetryClient { get; set; }

        /// <summary>
        /// Gets or sets the (potential) set of Qna Services used.
        /// This sample only uses a single Qna instance.
        /// </summary>
        /// <value>
        /// A QnaMaker client instance created based on configuration in the .bot file.
        /// </value>
        public Dictionary<string, QnAMaker> QnAServices { get; set; }
    }
}