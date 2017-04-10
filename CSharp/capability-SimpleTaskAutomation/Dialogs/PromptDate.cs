namespace SimpleTaskAutomationBot.Dialogs
{
    using System;
    using System.Globalization;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Dialogs.Internals;
    using Microsoft.Bot.Connector;

    [Serializable]
    public class PromptDate : Prompt<DateTime, string>
    {
        public PromptDate(string prompt, string retry = null, string tooManyAttempts = null, int attempts = 3)
          : base(new PromptOptions<string>(prompt, retry, tooManyAttempts, attempts: attempts))
        {
        }

        protected override bool TryParse(IMessageActivity message, out DateTime result)
        {
            var quitCondition = message.Text.Equals("Cancel", StringComparison.InvariantCultureIgnoreCase);

            return DateTime.TryParseExact(message.Text, "MM/dd/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out result) || quitCondition;
        }
    }
}