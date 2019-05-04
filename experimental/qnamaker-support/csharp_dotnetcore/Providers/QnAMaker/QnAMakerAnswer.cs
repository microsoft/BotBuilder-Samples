// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace SupportBot.Providers.QnAMaker
{
    using System.Collections.Generic;

    /// <summary>
    /// Holds all relevant information for QnaMaker Answer.
    /// </summary>
    public class QnAMakerAnswer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="QnAMakerAnswer"/> class.
        /// </summary>
        public QnAMakerAnswer()
        {
            Text = null;
            Requery = null;
            Options = new List<PromptOption>();
            IsRoot = "yes";
            Parent = null;
            Name = null;
            Flowtype = null;
            IsChitChat = false;
            QnAId = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="QnAMakerAnswer"/> class.
        /// </summary>
        /// <param name="text">Answeer text.</param>
        /// <param name="requery">Requery text.</param>
        /// <param name="options">Options.</param>
        public QnAMakerAnswer(string text, string requery = null, List<PromptOption> options = null)
        {
            Text = text;
            Requery = requery;
            Options = options ?? new List<PromptOption>();
            IsChitChat = false;
        }

        /// <summary>
        /// Gets or sets Answer text.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Gets or sets requery text.
        /// </summary>
        public string Requery { get; set; }

        /// <summary>
        /// Gets or sets list of prompt options.
        /// </summary>
        public List<PromptOption> Options { get; set; }

        /// <summary>
        /// Gets or sets Parent metadata value.
        /// </summary>
        public string Parent { get; set; }

        /// <summary>
        /// Gets or sets Name metadata value.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets IsRoot metadata value.
        /// </summary>
        public string IsRoot { get; set; }

        /// <summary>
        /// Gets or sets Flowtype metadata value.
        /// </summary>
        public string Flowtype { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether chitchat is triggered.
        /// </summary>
        public bool IsChitChat { get; set; }

        /// <summary>
        /// Gets or sets QnAId.
        /// </summary>
        public int QnAId { get; set; }

        /// <summary>
        /// Compares if answers are equal.
        /// </summary>
        /// <param name="answer">QnAMaker answer.</param>
        /// <returns>true if equal.</returns>
        public bool IsEqual(QnAMakerAnswer answer)
        {
            var isEqual = true;
            if (!answer.Text.Equals(this.Text))
            {
                isEqual = false;
            }

            foreach (var currentoption in answer.Options)
            {
                if (!this.Options.Exists(s => s.Option.Equals(currentoption.Option)))
                {
                    isEqual = false;
                }
            }

            return isEqual;
        }
    }
}
