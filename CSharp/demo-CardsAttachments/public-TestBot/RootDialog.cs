namespace TestBot
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Helpers;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Dialogs.Internals;
    using Microsoft.Bot.Connector;

    [Serializable]
    public class RootDialog : IDialog<object>
    {
        private readonly IEnumerable<string> validKeywords;

        public RootDialog(IEnumerable<string> validKeywords)
        {
            this.validKeywords = validKeywords;
        }

        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(this.MessageReceivedAsync);
        }

        public async virtual Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

            // not recognized command
            var reply = context.MakeMessage();

            reply.Text = $"Sorry {message.From.Name}, I cannot understand your message, the valid keywords are: {string.Join(", ", this.validKeywords.Select(kw => $"**{kw}**"))}";

            await context.PostAsync(reply);

            context.Wait(this.MessageReceivedAsync);
        }
    }
}