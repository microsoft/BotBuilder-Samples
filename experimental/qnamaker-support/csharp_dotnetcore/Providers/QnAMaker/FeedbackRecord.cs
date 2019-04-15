// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace SupportBot.Providers.QnAMaker
{
    /// <summary>
    /// Active learning feedback record.
    /// </summary>
        public class FeedbackRecord
    {
        /// <summary>
        /// Gets or sets User id.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets User question.
        /// </summary>
        public string UserQuestion { get; set; }

        /// <summary>
        /// Gets or sets QnA Id.
        /// </summary>
        public int QnaId { get; set; }
    }
}
