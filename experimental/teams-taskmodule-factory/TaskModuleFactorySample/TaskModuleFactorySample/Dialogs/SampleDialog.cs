// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;

namespace TaskModuleFactorySample.Dialogs
{
    public class SampleDialog : SkillDialogBase
    {
        public SampleDialog(
            IServiceProvider serviceProvider)
            : base(nameof(SampleDialog), serviceProvider)
        {
            var sample = new WaterfallStep[]
            {
                // NOTE: Uncomment these lines to include authentication steps to this dialog
                // GetAuthTokenAsync,
                // AfterGetAuthTokenAsync,
                PromptForNameAsync,
                GreetUserAsync,
                EndAsync,
            };

            AddDialog(new WaterfallDialog(nameof(SampleDialog), sample));
            AddDialog(new TextPrompt(DialogIds.NamePrompt));

            InitialDialogId = nameof(SampleDialog);
        }

        private async Task<DialogTurnResult> PromptForNameAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // NOTE: Uncomment the following lines to access LUIS result for this turn.
            // var luisResult = stepContext.Context.TurnState.Get<LuisResult>(StateProperties.SkillLuisResult);
            var prompt = TemplateEngine.GenerateActivityForLocale("NamePrompt");
            return await stepContext.PromptAsync(DialogIds.NamePrompt, new PromptOptions { Prompt = prompt }, cancellationToken);
        }

        private async Task<DialogTurnResult> GreetUserAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            dynamic data = new { Name = stepContext.Result.ToString() };
            var response = TemplateEngine.GenerateActivityForLocale("HaveNameMessage", data);
            await stepContext.Context.SendActivityAsync(response);

            return await stepContext.NextAsync(cancellationToken: cancellationToken);
        }

        private Task<DialogTurnResult> EndAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            return stepContext.EndDialogAsync(cancellationToken: cancellationToken);
        }

        private static class DialogIds
        {
            public const string NamePrompt = "namePrompt";
        }
    }
}
