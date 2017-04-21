using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Bot.Dialogs
{
    [Serializable]
    public class PayDialog : IDialog<object>
    {
        private const string NOTICES_STATE_BAG_KEY = @"notices";

        private static readonly Regex NOTICE_VALIDATOR = new Regex("^[A-Za-z0-9]{10}$", RegexOptions.Compiled);
        private static readonly List<string> EXIT_COMMANDS = new List<string> { "start over", "done", "exit", "quit", "reset" };

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public async Task StartAsync(IDialogContext context)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            context.Wait(EnterPaymentAmount);
        }

        private async Task EnterPaymentAmount(IDialogContext context, IAwaitable<object> result)
        {
            await PromptForAmountToPay(context, @"I'd be happy to help you give me money!<br/>
Please enter the amount you wish to pay.");
        }

        private async Task PromptForAmountToPay(IDialogContext context, string promptText)
        {
            // Can't use PromptDialog.Text because we want to drop a ResumptionCookie when we get the user's response.
            // This requires IAwaitable<IMessageActivity>
            var msg = context.MakeMessage();
            msg.Text = promptText;

            await context.PostAsync(msg);

            context.Wait(PaymentAmountEntered);
        }

        private async Task PaymentAmountEntered(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var content = await result;

            double pmtAmount;
            if (!double.TryParse(content.Text, out pmtAmount))
            {
                await context.PostAsync(@"Sorry, doesn't look like a valid payment amount. Try again!");
                context.Wait(PaymentAmountEntered);
            }
            else
            {
                // Drop a resumption cookie we'll pick back up after the user pays on Paypal
                var resumption = new ResumptionCookie(content);
                context.PrivateConversationData.SetValue("resumption", resumption);

                await context.Forward(new PayFlow(), PaymentComplete, content.Text, System.Threading.CancellationToken.None);
            }
        }

        private async Task PaymentComplete(IDialogContext context, IAwaitable<bool> result)
        {
            if (await result)
            {
                BeDone(context);
            }
            else
            {
                await PromptForAmountToPay(context, @"Oops! There appears to have been an error processing your payment.<br/>
Enter another amount to try again.");
            }
        }

        private void BeDone(IDialogContext context, bool result = true)
        {
            context.ConversationData.RemoveValue(NOTICES_STATE_BAG_KEY);
            context.Done(result);
        }
    }
}