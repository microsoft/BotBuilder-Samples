// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Newtonsoft.Json.Linq;

namespace BasicBot
{
    /// <summary>
    /// Demonstrates the following concepts:
    /// - Use a subclass of ComponentDialog to implement a multi-turn conversation
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
        /// <param name="botState">The <see cref="UserState"/> for storing properties at user-scope.</param>
        public GreetingDialog(BotServices botServices, UserState botState)
            : this(botServices, botState.CreateProperty<GreetingState>(GreetingStateName))
        {
        }

        public GreetingDialog(BotServices botServices, IStatePropertyAccessor<GreetingState> greetingStateAccessor)
            : base(botServices, nameof(GreetingDialog))
        {
            this.GreetingStateAccessor = greetingStateAccessor;

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

        /// <summary>
        /// Gets the <see cref="IStatePropertyAccessor{T}"/> name used for the <see cref="CounterState"/> accessor.
        /// </summary>
        /// <remarks>Accessors require a unique name.</remarks>
        /// <value>The accessor name for the accessor.</value>
        public static string GreetingStateName { get; } = $"{nameof(GreetingDialog)}.GreetingState";

        public IStatePropertyAccessor<GreetingState> GreetingStateAccessor { get; }

        private BotServices BotServices { get; }

        // Handle updates to entities.
        protected override async Task<bool> ProcessUpdateEntitiesAsync(JObject entities, DialogContext dc, CancellationToken cancellationToken)
        {
            var greetingState = await GreetingStateAccessor.GetAsync(dc.Context, () => new GreetingState());

            // Supported LUIS Entities
            string[] userNameEntities = { "userName", "userName_patternAny" };
            string[] userLocationEntities = { "userLocation", "userLocation_patternAny" };

            var result = false;

            if (entities != null && entities.HasValues)
            {
                // Update any entities
                foreach (var name in userNameEntities)
                {
                    // check if we found valid slot values in entities returned from LUIS
                    if (entities[name] != null)
                    {
                        greetingState.Name = (string)entities[name][0];
                        result = true;
                        break;
                    }
                }

                foreach (var city in userLocationEntities)
                {
                    if (entities[city] != null)
                    {
                        greetingState.City = (string)entities[city][0];
                        result = true;
                        break;
                    }
                }

                // set the new values
                await GreetingStateAccessor.SetAsync(dc.Context, greetingState);
            }

            return result;
        }

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
            var greetingState = await GreetingStateAccessor.GetAsync(stepContext.Context, () => new GreetingState());
            var args = stepContext.Result as string;
            if (!string.IsNullOrWhiteSpace(args) && string.IsNullOrWhiteSpace(greetingState.Name))
            {
                greetingState.Name = args;
            }

            // Prompt for city if missing
            if (string.IsNullOrWhiteSpace(greetingState.City))
            {
                var options = new PromptOptions
                {
                    Prompt = new Activity
                    {
                        Type = ActivityTypes.Message,
                        Text = $"What city do you live in?",
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
            var greeting = await GreetingStateAccessor.GetAsync(stepContext.Context, () => new GreetingState());

            var args = stepContext.Result as string;
            if (string.IsNullOrWhiteSpace(greeting.City) && !string.IsNullOrWhiteSpace(args))
            {
                greeting.City = args;
                await GreetingStateAccessor.SetAsync(stepContext.Context, greeting);
                await stepContext.Context.SendActivityAsync($"Ok `{greeting.Name}`, I've got you living in `{greeting.City}`.");
                await stepContext.Context.SendActivityAsync("I'll go ahead an update your profile with that information.");
            }
            else
            {
                await stepContext.Context.SendActivityAsync($"Hi `{greeting.Name}`, living in `{greeting.City}`,"
                    + " I understand greetings and asking for help!  Or start your connection over for a card.");
            }

            return await stepContext.EndDialogAsync();
        }
    }
}
