// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace QnAMakerSample.Utils
{
    /// <summary>
    /// QnA dialog response options class.
    /// </summary>
    public class QnADialogResponseOptions
    {
        /// <summary>
        /// Gets or sets get or set for No answer.
        /// </summary>
        public string NoAnswer { get; set; }

        /// <summary>
        /// Gets or sets get or set for Active learning card title.
        /// </summary>
        public string ActiveLearningCardTitle { get; set; }

        /// <summary>
        /// Gets or sets get or set for Card no match text.
        /// </summary>
        public string CardNoMatchText { get; set; }

        /// <summary>
        /// Gets or sets get or set for Card no match response.
        /// </summary>
        public string CardNoMatchResponse { get; set; }
    }
}
