// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace SupportBot.Providers.QnAMaker
{
    /// <summary>
    /// Prompt option.
    /// </summary>
    public class PromptOption
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PromptOption"/> class.
        /// </summary>
        public PromptOption()
        {
            Option = null;
            Requery = null;
            QnAId = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PromptOption"/> class.
        /// </summary>
        /// <param name="option">option text.</param>
        /// <param name="requery">requesry text.</param>
        /// <param name="qnaId">qna ID.</param>
        public PromptOption(string option, string requery = null, int qnaId = 0)
        {
            Option = option;
            Requery = requery;
            QnAId = qnaId;
        }

        /// <summary>
        /// Gets or sets Option text.
        /// </summary>
        public string Option { get; set; }

        /// <summary>
        /// Gets or sets Requery text.
        /// </summary>
        public string Requery { get; set; }

        /// <summary>
        /// Gets or sets QnA Id - required for train api call.
        /// </summary>
        public int QnAId { get; set; }
    }
}
