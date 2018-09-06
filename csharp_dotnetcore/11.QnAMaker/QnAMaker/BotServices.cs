// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License

using System;
using System.Collections.Generic;
using Microsoft.Bot.Builder.AI.QnA;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// Represents references to external services.
    ///
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
        public BotServices(Dictionary<string, QnAMaker> qnaServices)
        {
            QnAServices = qnaServices ?? throw new ArgumentNullException(nameof(qnaServices));
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
        public Dictionary<string, QnAMaker> QnAServices { get; } = new Dictionary<string, QnAMaker>();
    }
}