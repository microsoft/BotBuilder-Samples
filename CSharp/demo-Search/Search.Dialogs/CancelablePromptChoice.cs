namespace Search.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Connector;

    [Serializable]
    public class CancelablePromptChoice<T> : PromptDialog.PromptChoice<T>
    {
        protected readonly CancelablePromptOptions<T> PromptOptions;

        private static IEnumerable<string> cancelTerms = new[] { "Cancel", "Back", "B", "Abort" };

        public CancelablePromptChoice(CancelablePromptOptions<T> promptOptions)
            : base(promptOptions)
        {
            this.PromptOptions = promptOptions;
        }

        public CancelablePromptChoice(IEnumerable<T> options, string prompt, string cancelPrompt, string retry, int attempts, PromptStyle promptStyle = PromptStyle.Auto)
            : this(new CancelablePromptOptions<T>(prompt, cancelPrompt, retry, options: options.ToList(), attempts: attempts, promptStyler: new PromptStyler(promptStyle)))
        {
        }

        public static void Choice(IDialogContext context, ResumeAfter<T> resume, IEnumerable<T> options, string prompt, string cancelPrompt = null, string retry = null, int attempts = 3, PromptStyle promptStyle = PromptStyle.Auto)
        {
            Choice(context, resume, new CancelablePromptOptions<T>(prompt, cancelPrompt, retry, attempts: attempts, options: options.ToList(), promptStyler: new PromptStyler(promptStyle)));
        }

        public static void Choice(IDialogContext context, ResumeAfter<T> resume, CancelablePromptOptions<T> promptOptions)
        {
            var child = new CancelablePromptChoice<T>(promptOptions);
            context.Call(child, resume);
        }

        public static bool IsCancel(string text)
        {
            return cancelTerms.Any(t => string.Equals(t, text, StringComparison.CurrentCultureIgnoreCase));
        }

        protected override bool TryParse(IMessageActivity message, out T result)
        {
            if (IsCancel(message.Text))
            {
                result = default(T);
                return true;
            }

            return base.TryParse(message, out result);
        }

        protected override IMessageActivity MakePrompt(IDialogContext context, string prompt, IReadOnlyList<T> options = null, IReadOnlyList<string> descriptions = null, string speak = null)
        {
            prompt += Environment.NewLine + (this.PromptOptions.CancelPrompt ?? this.PromptOptions.DefaultCancelPrompt);
            return base.MakePrompt(context, prompt, options, descriptions, speak);
        }
    }
}