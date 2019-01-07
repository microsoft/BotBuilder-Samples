// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License

using System;
using System.Collections.Generic;
<<<<<<< HEAD
using Microsoft.ApplicationInsights;
=======
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
using Microsoft.Bot.Builder.AI.Luis;
using Microsoft.Bot.Builder.AI.QnA;

namespace NLP_With_Dispatch_Bot
{
    /// <summary>
    /// Represents the bot's references to external services.
    ///
    /// For example, Application Insights, Luis models and QnaMaker services
<<<<<<< HEAD
    /// are kept here (singletons).  These external services are configured
=======
    /// are kept here (singletons). These external services are configured
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
    /// using the BotConfigure class (based on the contents of your ".bot" file).
    /// </summary>
    public class BotServices
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BotServices"/> class.
        /// </summary>
        /// <param name="client">An Application Insights <see cref="TelemetryClient"/> instance.</param>
        /// <param name="qnaServices">A dictionary of named <see cref="QnAMaker"/> instances for usage within the bot.</param>
<<<<<<< HEAD
=======
        /// <param name="luisServices">A dictionary of named <see cref="LuisRecognizer"/> instances for usage within the bot</param>
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
        public BotServices(Dictionary<string, QnAMaker> qnaServices, Dictionary<string, LuisRecognizer> luisServices)
        {
            QnAServices = qnaServices ?? throw new ArgumentNullException(nameof(qnaServices));
            LuisServices = luisServices ?? throw new ArgumentNullException(nameof(luisServices));
        }

        /// <summary>
        /// Gets the (potential) set of QnA Services used.
        /// Given there can be multiple QnA services used in a single bot,
<<<<<<< HEAD
        /// QnA is represented as a Dictionary.  This is also modeled in the
=======
        /// QnA is represented as a Dictionary. This is also modeled in the
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
        /// ".bot" file since the elements are named (string).
        /// This sample only uses a single QnA instance.
        /// </summary>
        /// <value>
        /// A QnAMaker client instance created based on configuration in the .bot file.
        /// </value>
        public Dictionary<string, QnAMaker> QnAServices { get; } = new Dictionary<string, QnAMaker>();

        /// <summary>
        /// Gets the (potential) set of Luis Services used.
        /// Given there can be multiple Luis services used in a single bot,
<<<<<<< HEAD
        /// LuisServices is represented as a Dictionary.  This is also modeled in the
=======
        /// LuisServices is represented as a Dictionary. This is also modeled in the
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
        /// ".bot" file since the elements are named (string).
        /// This sample only uses a single Luis instance.
        /// </summary>
        /// <value>
        /// A <see cref="LuisRecognizer"/> client instance created based on configuration in the .bot file.
        /// </value>
        public Dictionary<string, LuisRecognizer> LuisServices { get; } = new Dictionary<string, LuisRecognizer>();
    }
<<<<<<< HEAD
}
=======
}
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
