// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License

namespace SupportBot.Service
{
    using System;
    using System.Collections.Generic;
    using Microsoft.ApplicationInsights;
    using Microsoft.Bot.Builder.AI.Luis;
    using Microsoft.Bot.Builder.AI.QnA;
    using SupportBot.Middleware.Telemetry;

    /// <summary>
    /// Represents references to external services.
    /// For example, the QnA Maker service is kept here as a singleton. This external service is configured
    /// using the <see cref="Microsoft.Bot.Configuration.BotConfiguration"/> class
    /// (based on the contents of your ".bot" file).
    /// </summary>
    public class BotServices
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BotServices"/> class.
        /// </summary>
        /// <param name="qnaServices">A dictionary of named <see cref="QnAMaker"/> instances for usage within the bot.</param>
        public BotServices(Dictionary<string, TelemetryQnaMaker> qnaServices, Dictionary<string, LuisRecognizer> luisServices, TelemetryClient telemetryClient)
        {
            QnAServices = qnaServices ?? throw new ArgumentNullException(nameof(qnaServices));
            LuisServices = luisServices ?? throw new ArgumentNullException(nameof(luisServices));
            TelemetryClient = telemetryClient ?? throw new ArgumentNullException(nameof(telemetryClient));

        }

        /// <summary>
        /// Gets the set of QnA Maker services used.
        /// Given there can be multiple <see cref="QnAMaker"/> services used in a single bot,
        /// QnA Maker instances are represented as a Dictionary.  This is also modeled in the
        /// ".bot" file using named elements.
        /// </summary>
        /// <remarks>The QnA Maker services collection should not be modified while the bot is running.</remarks>
        /// <value>
        /// A <see cref="QnAMaker"/> client instance created based on configuration in the .bot file.
        /// </value>
        public Dictionary<string, TelemetryQnaMaker> QnAServices { get; } = new Dictionary<string, TelemetryQnaMaker>();

        /// <summary>
        /// Gets the set of LUIS services used.
        /// Given there can be multiple <see cref="LuisServices"/> services used in a single bot,
        /// LUIS service instances are represented as a Dictionary.  This is also modeled in the
        /// ".bot" file using named elements.
        /// </summary>
        /// <remarks>The LUIS services collection should not be modified while the bot is running.</remarks>
        /// <value>
        /// A <see cref="LuisRecognizer"/> client instance created based on configuration in the .bot file.
        /// </value>
        public Dictionary<string, LuisRecognizer> LuisServices { get; } = new Dictionary<string, LuisRecognizer>();

        public TelemetryClient TelemetryClient { get; set; }
    }
}
