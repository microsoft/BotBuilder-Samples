namespace ContosoFlowers.BotAssets.Dialogs
{
    using System;
    using System.Text.RegularExpressions;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Dialogs.Internals;
    using Microsoft.Bot.Connector;
    using Properties;

    [Serializable]
    public class PromptStringRegex : Prompt<string, string>
    {
        private readonly Regex regex;

        public PromptStringRegex(string prompt, string regexPattern, string retry = null)
            : base(new PromptOptions<string>(prompt, retry))
        {
            this.regex = new Regex(regexPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);
        }

        protected override bool TryParse(IMessageActivity message, out string result)
        {
            var quitCondition = message.Text.Equals("B", StringComparison.InvariantCultureIgnoreCase) || message.Text.Equals("Back", StringComparison.InvariantCultureIgnoreCase);
            var validEmail = this.regex.Match(message.Text).Success;

            result = validEmail ? message.Text : null;

            return validEmail || quitCondition;
        }
    }
}