// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Collections.Generic;
using Microsoft.Bot.Builder.BotBuilderSamples;

// SINCE ADAPTIVECARDPROMPT ISN'T YET IN THE SDK, EVERYTHING IN RemoveOncePromptInSDK WILL BE DELETED AND THIS WILL BE REMOVED FROM THE SAMPLE

namespace Microsoft.Bot.Builder.Dialogs
{
    /// <summary>
    /// Represents a result from adaptive card input.
    /// </summary>
    /// <typeparam name="T">Type returned by recognizer.</typeparam>
    public class AdaptiveCardPromptResult
    {
        /// <summary>
        /// Gets or sets the Value of the Adaptive Card input.
        /// </summary>
        /// <value>
        /// The value of the user's input from the Adaptive Card.
        /// </value>
        public object Data { get; set; }

        /// <summary>
        /// Gets or sets Error enum.
        /// </summary>
        /// <value>
        /// If not recognized.succeeded, include reason why, if known.
        /// </value>
        public AdaptiveCardPromptErrors Error { get; set; }

        /// <summary>
        /// Gets or sets array of missing required Ids.
        /// </summary>
        /// <value>
        /// Array of requiredIds that were not included with user input.
        /// </value>
        public List<string> MissingIds { get; set; }
    }
}
