// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;

namespace ScaleoutBot
{
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
            return new WaterfallDialog("root-waterfall", new WaterfallStep[] { Step1, Step2, Step3 });
        }

        private static async Task<DialogTurnResult> Step1(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return await stepContext.PromptAsync("number", new PromptOptions { Prompt = MessageFactory.Text("Enter a number.") }, cancellationToken);
        }
        private static async Task<DialogTurnResult> Step2(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var first = (long)stepContext.Result;
            stepContext.Values["first"] = first;
            return await stepContext.PromptAsync("number", new PromptOptions { Prompt = MessageFactory.Text($"I have {first} now enter another number") }, cancellationToken);
        }
        private static async Task<DialogTurnResult> Step3(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var first = (long)stepContext.Values["first"];
            var second = (long)stepContext.Result;
            await stepContext.Context.SendActivityAsync(MessageFactory.Text($"The result of the first minus the second is {first - second}."), cancellationToken);
            return await stepContext.EndDialogAsync(cancellationToken);
        }
    }
}
