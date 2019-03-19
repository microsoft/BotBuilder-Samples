// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

namespace Microsoft.BotBuilderSamples
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Bot.Builder;
    using Microsoft.Bot.Builder.Dialogs;
    using Microsoft.Bot.Builder.Dialogs.Choices;
    using Microsoft.Extensions.Logging;

    public class MainDialog : ComponentDialog
    {
        // Define a "done" response for the company selection prompt.
        private const string DoneOption = "done";

        // Define value names for values tracked inside the dialogs.
        private const string UserInfo = "value-userInfo";
        private const string CompaniesSelected = "value-companiesSelected";

        // Define the company choices for the company selection prompt.
        private readonly string[] _companyOptions = new string[]
        {
            "Adatum Corporation", "Contoso Suites", "Graphic Design Institute", "Wide World Importers",
        };

        private readonly ILogger<MainDialog> _logger;

        public MainDialog(ILogger<MainDialog> logger)
            : base(nameof(MainDialog))
        {
            _logger = logger;

            AddDialog(new TextPrompt("NamePrompt"));
            AddDialog(new NumberPrompt<int>("AgePrompt"));
            AddDialog(new ChoicePrompt("SelectionPrompt"));

            AddDialog(new WaterfallDialog("TopLevelDialog", new WaterfallStep[]
                {
                    NameStepAsync,
                    AgeStepAsync,
                    StartSelectionStepAsync,
                    AcknowledgementStepAsync,
                }));

            AddDialog(new WaterfallDialog("ReviewSelectionDialog", new WaterfallStep[]
                {
                    SelectionStepAsync,
                    LoopStepAsync,
                }));

            InitialDialogId = "TopLevelDialog";
        }

        private static async Task<DialogTurnResult> NameStepAsync(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            // Create an object in which to collect the user's information within the dialog.
            stepContext.Values[UserInfo] = new UserProfile();

            // Ask the user to enter their name.
            return await stepContext.PromptAsync(
                "NamePrompt",
                new PromptOptions { Prompt = MessageFactory.Text("Please enter your name.") },
                cancellationToken);
        }

        private async Task<DialogTurnResult> AgeStepAsync(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            // Set the user's name to what they entered in response to the name prompt.
            ((UserProfile)stepContext.Values[UserInfo]).Name = (string)stepContext.Result;

            // Ask the user to enter their age.
            return await stepContext.PromptAsync(
                "AgePrompt",
                new PromptOptions { Prompt = MessageFactory.Text("Please enter your age.") },
                cancellationToken);
        }

        private async Task<DialogTurnResult> StartSelectionStepAsync(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            // Set the user's age to what they entered in response to the age prompt.
            var age = (int)stepContext.Result;
            ((UserProfile)stepContext.Values[UserInfo]).Age = age;

            if (age < 25)
            {
                // If they are too young, skip the review selection dialog, and pass an empty list to the next step.
                await stepContext.Context.SendActivityAsync(
                    MessageFactory.Text("You must be 25 or older to participate."),
                    cancellationToken);
                return await stepContext.NextAsync(new List<string>(), cancellationToken);
            }
            else
            {
                // Otherwise, start the review selection dialog.
                return await stepContext.BeginDialogAsync("ReviewSelectionDialog", null, cancellationToken);
            }
        }

        private async Task<DialogTurnResult> AcknowledgementStepAsync(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            // Set the user's company selection to what they entered in the review-selection dialog.
            var list = stepContext.Result as List<string>;
            ((UserProfile)stepContext.Values[UserInfo]).CompaniesToReview = list ?? new List<string>();

            // Thank them for participating.
            await stepContext.Context.SendActivityAsync(
                MessageFactory.Text($"Thanks for participating, {((UserProfile)stepContext.Values[UserInfo]).Name}."),
                cancellationToken);

            // Exit the dialog, returning the collected user information.
            return await stepContext.EndDialogAsync(stepContext.Values[UserInfo], cancellationToken);
        }

        private async Task<DialogTurnResult> SelectionStepAsync(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            // Continue using the same selection list, if any, from the previous iteration of this dialog.
            var list = stepContext.Options as List<string> ?? new List<string>();
            stepContext.Values[CompaniesSelected] = list;

            // Create a prompt message.
            string message;
            if (list.Count is 0)
            {
                message = $"Please choose a company to review, or `{DoneOption}` to finish.";
            }
            else
            {
                message = $"You have selected **{list[0]}**. You can review an additional company, " +
                    $"or choose `{DoneOption}` to finish.";
            }

            // Create the list of options to choose from.
            var options = _companyOptions.ToList();
            options.Add(DoneOption);
            if (list.Count > 0)
            {
                options.Remove(list[0]);
            }

            // Prompt the user for a choice.
            return await stepContext.PromptAsync(
                "SelectionPrompt",
                new PromptOptions
                {
                    Prompt = MessageFactory.Text(message),
                    RetryPrompt = MessageFactory.Text("Please choose an option from the list."),
                    Choices = ChoiceFactory.ToChoices(options),
                },
                cancellationToken);
        }

        private async Task<DialogTurnResult> LoopStepAsync(
            WaterfallStepContext stepContext,
            CancellationToken cancellationToken)
        {
            // Retrieve their selection list, the choice they made, and whether they chose to finish.
            var list = stepContext.Values[CompaniesSelected] as List<string>;
            var choice = (FoundChoice)stepContext.Result;
            var done = choice.Value == DoneOption;

            if (!done)
            {
                // If they chose a company, add it to the list.
                list.Add(choice.Value);
            }

            if (done || list.Count >= 2)
            {
                // If they're done, exit and return their list.
                return await stepContext.EndDialogAsync(list, cancellationToken);
            }
            else
            {
                // Otherwise, repeat this dialog, passing in the list from this iteration.
                return await stepContext.ReplaceDialogAsync("ReviewSelectionDialog", list, cancellationToken);
            }
        }
    }
}
