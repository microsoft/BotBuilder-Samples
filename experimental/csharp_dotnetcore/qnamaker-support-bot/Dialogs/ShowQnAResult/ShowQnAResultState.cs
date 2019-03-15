// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace SupportBot.Dialogs.ShowQnAResult
{
    using SupportBot.Providers.QnAMaker;

    /// <summary>TurnContext State.</summary>
    public class ShowQnAResultState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShowQnAResultState"/> class.
        /// </summary>
        public ShowQnAResultState()
        {
            this.ConsiderState = false;
            this.QnaAnswer = new QnAMakerAnswer();
            this.NeuroconCount = 0;
            this.IsFeedback = false;
            this.ActiveLearningAnswer = false;
        }

        /// <summary>Gets or sets a value indicating whether context state needs to be considered.</summary>
        public bool ConsiderState { get; set; }

        /// <summary>Gets or sets QnAMaker answer.</summary>
        public QnAMakerAnswer QnaAnswer { get; set; }

        /// <summary>Gets or sets number of times personality chat or QnA chitchat answer is shown consequently.</summary>
        public int NeuroconCount { get; set; }

        /// <summary>Gets or sets a value indicating whether feedback flow is true.</summary>
        public bool IsFeedback { get; set; }

        /// <summary>Gets or sets a value indicating whether active learning is true.</summary>
        public bool ActiveLearningAnswer { get; set; }

        /// <summary>Gets or sets user question which invoked active learning.</summary>
        public string ActiveLearningUserQuestion { get; set; }
    }
}
