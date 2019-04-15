// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace SupportBot.Providers.QnAMaker
{
    using Newtonsoft.Json;

    /// <summary>
    /// Active learning feedback records.
    /// </summary>
    public class FeedbackRecords
    {
        /// <summary>
        /// Gets or sets List of feedback records.
        /// </summary>
        [JsonProperty("feedbackRecords")]
        public FeedbackRecord[] Records { get; set; }
    }
}
