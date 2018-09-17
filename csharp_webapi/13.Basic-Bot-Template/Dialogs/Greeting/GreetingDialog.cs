// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace BasicBot
{
    /// <summary>
    /// Demonstrates the following concepts:
    /// - Use a subclass of ComponentDialog to implement a mult-turn conversation
    /// - Use a Waterflow dialog to model multi-turn conversation flow
    /// - Use custom prompts to validate user input
    /// - Store conversation and user state.
    /// </summary>
    public class GreetingDialog : DetectHelpCancelDialog
    {
        // User state for greeting dialog
        private const string GreetingStateProperty = "greetingState";
        private const string NameValue = "greetingName";
        private const string CityValue = "greetingCity";

        // Dialog IDs
        private const string ProfileDialog = "profileDialog";

        /// <summary>
        /// Initializes a new instance of the <see cref="GreetingDialog"/> class.
        /// </summary>
        /// <param name="botServices">Connected services used in processing.</param>
        /// <param name="dialogStateAccessor">The <see cref="DialogState"/> property accessor.</param>
        /// <param name="greetingStateAccessor">The <see cref="GreetingState"/> property
        /// accessor for <see cref="UserState"/>. Used for holding name/city.</param>
        /// <param name="logger">The <see cref="ILogger"/> that enables logging.</param>
        public GreetingDialog(
                    BotServices botServices,
                    IStatePropertyAccessor<DialogState> dialogStateAccessor,
                    IStatePropertyAccessor<GreetingState> greetingStateAccessor)
            : base(botServices, nameof(GreetingDialog))
        {
            DialogStateAccessor = dialogStateAccessor ?? throw new ArgumentNullException($"Missing parameter. {nameof(dialogStateAccessor)} is required.");
            GreetingStateAccessor = greetingStateAccessor ?? throw new ArgumentNullException($"Missing parameter. {nameof(greetingStateAccessor)} is required.");

            // Add control flow dialogs
            var waterfallSteps = new WaterfallStep[]
            {
                InitializeStateStepAsync,
                PromptForNameStepAsync,
                PromptForCityStepAsync,
                DisplayGreetingStateStepAsync,
            };
            AddDialog(new WaterfallDialog(ProfileDialog, waterfallSteps));
            AddDialog(new NamePrompt(nameof(NamePrompt)));
            AddDialog(new CityPrompt(nameof(CityPrompt)));
        }

        public IStatePropertyAccessor<DialogState> DialogStateAccessor { get; }

        public IStatePropertyAccessor<GreetingState> GreetingStateAccessor { get; }

        private BotServices BotServices { get; }

        private async Task<DialogTurnResult> InitializeStateStepAsync(WaterfallStepContext stepContext, CancellationToken cancellationToken)
        {
            var greetingState = await GreetingStateAccessor.GetAsync(stepContext.Context, () => new GreetingState());

            stepContext.Values[NameValue] = greetingState.Name;
            stepContext.Values[CityValue] = greetingState.City;

            return await stepContext.NextAsync();
        }

        private async Task<DialogTurnResult> PromptForNameStepAsync(
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

                return await stepContext.PromptAsync(nameof(NamePrompt), options).ConfigureAwait(false);
            }
            else
            {
                return await stepContext.NextAsync();
            }
        }

        private async Task<DialogTurnResult> PromptForCityStepAsync(
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
                return await stepContext.PromptAsync(nameof(CityPrompt), options, cancellationToken);
            }
            else
            {
                return await stepContext.NextAsync();
            }
        }

        private async Task<DialogTurnResult> DisplayGreetingStateStepAsync(
                                                    WaterfallStepContext stepContext,
                                                    CancellationToken cancellationToken)
        {
            // Save city if it were prompted.
            var args = stepContext.Result as string;
            if (!string.IsNullOrWhiteSpace(args))
            {
                stepContext.Values[CityValue] = args;
                await stepContext.Context.SendActivityAsync($"Ok `{stepContext.Values[NameValue]}`, I've got you living in `{stepContext.Values[CityValue]}`.");
                await stepContext.Context.SendActivityAsync("I'll go ahead an update your profile with that information.");

                var greetingState = new GreetingState()
                {
                    City = stepContext.Values[CityValue] as string,
                    Name = stepContext.Values[NameValue] as string,
                };
                await GreetingStateAccessor.SetAsync(stepContext.Context, greetingState);
            }
            else
            {
                await stepContext.Context.SendActivityAsync($"Hi `{stepContext.Values[NameValue]}`, living in `{stepContext.Values[CityValue]}`,"
                    + " I understand greetings and asking for help!  Or start your connection over for a card.");
            }

            return await stepContext.EndDialogAsync();
        }
    }
}
