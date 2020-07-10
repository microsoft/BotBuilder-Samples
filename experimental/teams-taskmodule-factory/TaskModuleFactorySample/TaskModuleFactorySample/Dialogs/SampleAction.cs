// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Newtonsoft.Json;

namespace TaskModuleFactorySample.Dialogs
{
    public class SampleActionInput
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class SampleActionOutput
    {
        [JsonProperty("customerId")]
        public int CustomerId { get; set; }
    }

    public class SampleAction : SkillDialogBase
    {
        public SampleAction(
            IServiceProvider serviceProvider)
            : base(nameof(SampleAction), serviceProvider)
        {
            var sample = new WaterfallStep[]
            {
                PromptForNameAsync,
                GreetUserAsync,
                EndAsync,
            };

            AddDialog(new WaterfallDialog(nameof(SampleAction), sample));
            AddDialog(new TextPrompt(DialogIds.NamePrompt));

            InitialDialogId = nameof(SampleAction);
        }

        private async Task<DialogTurnResult> PromptForNameAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // If we have been provided a input data structure we pull out provided data as appropriate
            // and make a decision on whether the dialog needs to prompt for anything.
            if (stepContext.Options is SampleActionInput actionInput && !string.IsNullOrEmpty(actionInput.Name))
            {
                // We have Name provided by the caller so we skip the Name prompt.
                return await stepContext.NextAsync(actionInput.Name, cancellationToken);
            }

            var prompt = TemplateEngine.GenerateActivityForLocale("NamePrompt");
            return await stepContext.PromptAsync(DialogIds.NamePrompt, new PromptOptions { Prompt = prompt }, cancellationToken);
        }

        private async Task<DialogTurnResult> GreetUserAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            dynamic data = new { Name = stepContext.Result.ToString() };
            var response = TemplateEngine.GenerateActivityForLocale("HaveNameMessage", data);
            await stepContext.Context.SendActivityAsync(response);

            // Pass the response which we'll return to the user onto the next step
            return await stepContext.NextAsync(cancellationToken: cancellationToken);
        }

        private async Task<DialogTurnResult> EndAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            // Simulate a response object payload
            var actionResponse = new SampleActionOutput
            {
                CustomerId = new Random().Next()
            };

            // We end the dialog (generating an EndOfConversation event) which will serialize the result object in the Value field of the Activity
            return await stepContext.EndDialogAsync(actionResponse, cancellationToken);
        }

        private static class DialogIds
        {
            public const string NamePrompt = "namePrompt";
        }
    }
}