// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// This is an example root dialog. Replace this with your applications.
    /// </summary>
    public class RootDialog : ComponentDialog
    {
        public RootDialog()
            : base("root")
        {
            AddDialog(CreateWaterfall());
            AddDialog(new NumberPrompt<long>("number"));
        }

        private static WaterfallDialog CreateWaterfall()
        {
            return new WaterfallDialog("root-waterfall", new WaterfallStep[] { Step1Async, Step2Async, Step3Async });
        }

        private static async Task<DialogTurnResult> Step1Async(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync("number", new PromptOptions { Prompt = MessageFactory.Text("Enter a number.") }, cancellationToken);
        }

        private static async Task<DialogTurnResult> Step2Async(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var first = (long)stepContext.Result;
            stepContext.Values["first"] = first;
            return await stepContext.PromptAsync("number", new PromptOptions { Prompt = MessageFactory.Text($"I have {first} now enter another number") }, cancellationToken);
        }

        private static async Task<DialogTurnResult> Step3Async(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var first = (long)stepContext.Values["first"];
            var second = (long)stepContext.Result;
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"The result of the first minus the second is {first - second}."), cancellationToken);
            return await stepContext.EndDialogAsync(cancellationToken);
        }
    }
}
