// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples
{
    /// <summary>
    /// Demonstrates the following concepts:
    /// - Use a subclass of ComponentDialog to implement a mult-turn conversation
    /// - Use a Waterflow dialog to model multi-turn conversation flow
    /// - Use custom prompts to validate user input
    /// - Store conversation and user state
    /// </summary>
    public class GreetingDialog : ComponentDialog
    {
        // Prompt IDs
        public static readonly string NamePrompt = "namePrompt";
        public static readonly string CityPrompt = "cityPrompt";

        // User state for greeting dialog
        private const string GreetingStateProperty = "greetingState";
        private const string NameValue = "greetingName";
        private const string CityValue = "greetingCity";

        // Dialog IDs
        private const string ProfileDialog = "profileDialog";

        /// <summary>
        /// Initializes a new instance of the <see cref="GreetingDialog"/> class.
        /// </summary>
        /// <param name="dialogId">Unique identifier for this dialog instance.</param>
        /// <param name="dialogStateAccessor">The <see cref="DialogState"/> property accessor.</param>
        /// <param name="greetingStateAccessor">The <see cref="GreetingState"/> property
        /// accessor for <see cref="UserState"/>. Used for holding name/city.</param>
        public GreetingDialog(
                    string dialogId,
                    IStatePropertyAccessor<DialogState> dialogStateAccessor,
                    IStatePropertyAccessor<GreetingState> greetingStateAccessor)
            : base(dialogId)
        {
            // validate what was passed in
            if (string.IsNullOrWhiteSpace(dialogId))
            {
                throw new ArgumentNullException($"Missing parameter. {nameof(dialogId)} is required .");
            }

            DialogStateAccessor = dialogStateAccessor ?? throw new ArgumentNullException($"Missing parameter. {nameof(dialogStateAccessor)} is required.");
            GreetingStateAccessor = greetingStateAccessor ?? throw new ArgumentNullException($"Missing parameter. {nameof(greetingStateAccessor)} is required.");

            // Add control flow dialogs
            var profileSteps = new WaterfallStep[]
            {
                InitializeStateStepAsync,
                PromptForNameStepAsync,
                PromptForCityStepAsync,
                DisplayGreetingStateStepAsync,
            };
            AddDialog(new WaterfallDialog(ProfileDialog, profileSteps));
            AddDialog(new NamePrompt(GreetingDialog.NamePrompt));
            AddDialog(new CityPrompt(GreetingDialog.CityPrompt));
        }

        public IStatePropertyAccessor<DialogState> DialogStateAccessor { get; }

        public IStatePropertyAccessor<GreetingState> GreetingStateAccessor { get; }

        private async Task<DialogTurnResult> InitializeStateStepAsync(
                                                    DialogContext dc,
                                                    WaterfallStepContext stepContext,
                                                    CancellationToken cancellationToken)
        {

            var greetingState = await GreetingStateAccessor.GetAsync(dc.Context, () => new GreetingState());

            stepContext.Values[NameValue] = greetingState.Name;
            stepContext.Values[CityValue] = greetingState.City;

            return await stepContext.NextAsync();
        }

        private async Task<DialogTurnResult> PromptForNameStepAsync(
                                                DialogContext dc,
                                                WaterfallStepContext stepContext,
                                                CancellationToken cancellationToken)
        {
            // Prompt for name if missing
            if (string.IsNullOrWhiteSpace((string)stepContext.Values[NameValue]))
            {
                var options = new PromptOptions
                {
                    Prompt = new Activity
                    {
                        Type = ActivityTypes.Message,
                        Text = "What is your name ?",
                    },
                };

                return await dc.PromptAsync(NamePrompt, options).ConfigureAwait(false);
            }
            else
            {
                return await stepContext.NextAsync();
            }
        }

        private async Task<DialogTurnResult> PromptForCityStepAsync(
                                                        DialogContext dc,
                                                        WaterfallStepContext stepContext,
                                                        CancellationToken cancellationToken)
        {
            // Save name if prompted for
            var args = stepContext.Result as string;
            if (!string.IsNullOrWhiteSpace(args))
            {
                stepContext.Values[NameValue] = args;
            }

            // Prompt for city if missing
            if (string.IsNullOrWhiteSpace(stepContext.Values[CityValue] as string))
            {
                var options = new PromptOptions
                {
                    Prompt = new Activity
                    {
                        Type = ActivityTypes.Message,
                        Text = $"`{stepContext.Values[NameValue]}`, what city do you live in?",
                    },
                };
                return await dc.PromptAsync(CityPrompt, options, cancellationToken);
            }
            else
            {
                return await stepContext.NextAsync();
            }
        }

        private async Task<DialogTurnResult> DisplayGreetingStateStepAsync(
                                                    DialogContext dc,
                                                    WaterfallStepContext stepContext,
                                                    CancellationToken cancellationToken)
        {
            // Save city if it were prompted.
            var args = stepContext.Result as string;
            if (!string.IsNullOrWhiteSpace(args))
            {
                stepContext.Values[CityValue] = args;
                await dc.Context.SendActivityAsync($"Ok `{stepContext.Values[NameValue]}`, I've got you living in `{stepContext.Values[CityValue]}`.");
                await dc.Context.SendActivityAsync("I'll go ahead an update your profile with that information.");

                var greetingState = new GreetingState()
                {
                    City = stepContext.Values[CityValue] as string,
                    Name = stepContext.Values[NameValue] as string,
                };
                await GreetingStateAccessor.SetAsync(dc.Context, greetingState);
            }
            else
            {
                await dc.Context.SendActivityAsync($"Hi `{stepContext.Values[NameValue]}`, living in `{stepContext.Values[CityValue]}`,"
                    + " I understand greetings and asking for help!  Or start your connection over for a card.");
            }

            return await dc.EndAsync();
        }
    }
}