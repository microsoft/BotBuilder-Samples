// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License

using System;
using System.Collections.Generic;
<<<<<<< HEAD
using Microsoft.ApplicationInsights;
using Microsoft.Bot.Builder.AI.QnA;
=======
using Microsoft.Bot.Builder;
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// Represents the bot's references to external services.
    ///
    /// For example, Application Insights and QnaMaker services
    /// are configured here using the <see cref="Microsoft.Bot.Configuration.BotConfiguration"/> class
    /// (based on the contents of your ".bot" file).
    /// </summary>
    public class BotServices
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BotServices"/> class.
        /// </summary>
        /// <param name="client">An Application Insights <see cref="TelemetryClient"/> instance.</param>
<<<<<<< HEAD
        /// <param name="qnaServices">A dictionary of named <see cref="QnAMaker"/> instances for usage within the bot.</param>
        public BotServices(TelemetryClient client, Dictionary<string, QnAMaker> qnaServices)
        {
            TelemetryClient = client ?? throw new ArgumentNullException(nameof(client));
=======
        /// <param name="qnaServices">A dictionary of named <see cref="TelemetryQnAMaker"/> instances for usage within the bot.</param>
        public BotServices(Dictionary<string, TelemetryQnaMaker> qnaServices)
        {
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
            QnAServices = qnaServices ?? throw new ArgumentNullException(nameof(qnaServices));
        }

        /// <summary>
<<<<<<< HEAD
        /// Gets the Application Insights Telemetry client.
        /// Use this to log new custom events/metrics/traces/etc into your
        /// Application Insights service for later analysis.
        /// </summary>
        /// <value>
        /// The Application Insights <see cref="TelemetryClient"/> instance created based on configuration in the .bot file.
        /// </value>
        public TelemetryClient TelemetryClient { get; }

        /// <summary>
=======
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
        /// Gets the (potential) set of QnA Services used.
        /// Given there can be multiple QnA services used in a single bot,
        /// QnA is represented as a Dictionary.  This is also modeled in the
        /// ".bot" file since the elements are named (string).
        /// This sample only uses a single QnA instance.
        /// </summary>
        /// <value>
        /// A QnAMaker client instance created based on configuration in the .bot file.
        /// </value>
<<<<<<< HEAD
        public Dictionary<string, QnAMaker> QnAServices { get; } = new Dictionary<string, QnAMaker>();
    }
}
=======
        public Dictionary<string, TelemetryQnaMaker> QnAServices { get; } = new Dictionary<string, TelemetryQnaMaker>();
    }
}
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
