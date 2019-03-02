// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace Microsoft.BotBuilderSamples
{
    public class BookingDialog : ComponentDialog
    {
        public BookingDialog()
            : base("root")
        {
            AddDialog(new TextPrompt("text"));
            AddDialog(new ConfirmPrompt("confirm"));
            AddDialog(new WaterfallDialog("waterfall", new WaterfallStep[]
            {
                DestinationStepAsync,
                OriginStepAsync,
                TravelDateStepAsync,
                ConfirmStepAsync,
                FinalStepAsync,
            }));

            // The initial child Dialog to run.
            InitialDialogId = "waterfall";
        }

        private async Task<DialogTurnResult> DestinationStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync("text", new PromptOptions { Prompt = MessageFactory.Text("enter destination") }, cancellationToken);
        }

        private async Task<DialogTurnResult> OriginStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var desination = stepContext.Result;

            stepContext.Values["desination"] = desination;

            return await stepContext.PromptAsync("text", new PromptOptions { Prompt = MessageFactory.Text("enter origin") }, cancellationToken);
        }
        private async Task<DialogTurnResult> TravelDateStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var origin = stepContext.Result;

            stepContext.Values["origin"] = origin;

            return await stepContext.PromptAsync("text", new PromptOptions { Prompt = MessageFactory.Text("enter travel date") }, cancellationToken);
        }

        private async Task<DialogTurnResult> ConfirmStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var travelDate = stepContext.Result;

            stepContext.Values["travelDate"] = travelDate;

            return await stepContext.PromptAsync("confirm", new PromptOptions { Prompt = MessageFactory.Text("please confirm") }, cancellationToken);
        }

        private async Task<DialogTurnResult> FinalStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var msg = $"I have you booked to {stepContext.Values["desination"]} from {stepContext.Values["origin"]} on {stepContext.Values["travelDate"]}";

            await stepContext.Context.SendActivityAsync(MessageFactory.Text(msg), cancellationToken);

            // Remember to call EndAsync to indicate to the runtime that this is the end of our waterfall.
            return await stepContext.EndDialogAsync();
        }
    }
}
