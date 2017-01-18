namespace Search.Dialogs
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Bot.Builder.Dialogs;

    [Serializable]
    public class CancelablePromptOptions<T> : PromptOptions<T>
    {
        public CancelablePromptOptions(string prompt, string cancelPrompt = null, string retry = null, string tooManyAttempts = null, IReadOnlyList<T> options = null, int attempts = 3, PromptStyler promptStyler = null)
            : base(prompt, retry, tooManyAttempts, options, attempts, promptStyler)
        {
            this.DefaultCancelPrompt = "You can type Cancel or (B)ack or Abort to return to abandon this dialog.";

            this.CancelPrompt = cancelPrompt;
        }

        public string DefaultCancelPrompt { get; private set; }

        public string CancelPrompt { get; private set; }
    }
}