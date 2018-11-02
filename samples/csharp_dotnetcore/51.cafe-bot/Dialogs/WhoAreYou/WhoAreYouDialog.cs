// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;

namespace Microsoft.BotBuilderSamples
{
    public class WhoAreYouDialog : ComponentDialog
    {
        // This dialog's name. Matches the name of the LUIS intent from ../dispatcher/resources/cafeDispatchModel.lu
        // LUIS recognizer replaces spaces " " with "_". So intent name "Who are you" is recognized as "Who_are_you".
        public const string Name = "Who_are_you";

        // User name entity from ../whoAreYou/resources/whoAreYou.lu
        private readonly string userNameEntity = "userName";

        private readonly string userNamePatternAnyEntity = "userName_patternAny";

        // Names for dialogs and prompts
        private readonly string askUserNamePrompt = "askUserNamePrompt";
        private readonly string dialogStart = "Who_are_you_start";
        private readonly string confirmCancelPrompt = "confirmCancelPrompt";

        private readonly bool haveUserName = true;

        /**
           * Constructor.
           *
           * @param {BotConfiguration} bot configuration
           * @param {ConversationState} conversationState
           * @param {StatePropertyAccessor} accessor for user profile property
           * @param {StatePropertyAccessor} accessor for on turn property
           * @param {StatePropertyAccessor} accessor for reservation property
           */
        public WhoAreYouDialog(
                        BotServices botServices,
                        ConversationState conversationState,
                        IStatePropertyAccessor<UserProfile> userProfileAccessor,
                        IStatePropertyAccessor<OnTurnProperty> onTurnAccessor,
                        IStatePropertyAccessor<ReservationProperty> reservationAccessor)
        : base(Name)
        {
            if (botServices == null)
            {
                throw new ArgumentNullException(nameof(botServices));
            }

            if (conversationState == null)
            {
                throw new ArgumentNullException(nameof(conversationState));
            }

            UserProfileAccessor = userProfileAccessor ?? throw new ArgumentNullException(nameof(userProfileAccessor));

            // Keep accessors for the steps to consume
            OnTurnAccessor = onTurnAccessor ?? throw new ArgumentNullException(nameof(onTurnAccessor));

            // Add dialogs
            var waterfallSteps = new WaterfallStep[]
            {
                AskForUserNameAsync,
                GreetUserAsync,
            };
            AddDialog(new WaterfallDialog(
               dialogStart,
               waterfallSteps));

            var turnCounterAccessor = conversationState.CreateProperty<CounterState>("turnCounter");

            // Add get user name prompt.
            AddDialog(new GetUserNamePrompt(
                askUserNamePrompt,
                botServices,
                userProfileAccessor,
                conversationState,
                onTurnAccessor,
                turnCounterAccessor,
                async (promptContext, cancellationToken) =>
                {
                    var userProfile = await userProfileAccessor.GetAsync(promptContext.Context);
                    var counter = await turnCounterAccessor.GetAsync(promptContext.Context);

                    // Prompt validator
                    // Examine if we have a user name and validate it.
                    if (userProfile != null && userProfile.UserName != null)
                    {
                        // We can only accept user names that up to two words.
                        if (userProfile.UserName.Split(" ").Length > 2)
                        {
                            await promptContext.Context.SendActivityAsync("Sorry, I can only accept two words for a name.");
                            await promptContext.Context.SendActivityAsync("You can always say 'My name is <your name>' to introduce yourself to me.");
                            await userProfileAccessor.SetAsync(promptContext.Context, new UserProfile("Human"));

                            // Set updated turn counter
                            await turnCounterAccessor.SetAsync(promptContext.Context, counter);
                            return false;
                        }
                        else
                        {
                            // Capitalize user name
                            userProfile.UserName = char.ToUpper(userProfile.UserName[0]) + userProfile.UserName.Substring(1);

                            // Create user profile and set it to state.
                            await userProfileAccessor.SetAsync(promptContext.Context, userProfile);
                            return true;
                        }
                    }

                    return false;
                }));

            // This dialog is interruptable, add interruptionDispatcherDialog
            AddDialog(new InterruptionDispatcher(onTurnAccessor, conversationState, userProfileAccessor, botServices));

            // When user decides to abandon this dialog, we need to confirm user action - add confirmation prompt
            AddDialog(new ConfirmPrompt(confirmCancelPrompt));
        }

        public IStatePropertyAccessor<UserProfile> UserProfileAccessor { get; }

        public IStatePropertyAccessor<OnTurnProperty> OnTurnAccessor { get; }

        /**
         * Waterfall step to prompt for user's name
         *
         * @param {DialogContext} Dialog context
         * @param {WaterfallStepContext} water fall step context
         */
        private async Task<DialogTurnResult> AskForUserNameAsync(
                                                WaterfallStepContext stepContext,
                                                CancellationToken cancellationToken)
        {
            var context = stepContext.Context;

            // Get user profile.
            var userProfile = await UserProfileAccessor.GetAsync(context, () => new UserProfile(null, null));

            // Get on turn properties.
            var onTurnProperty = await OnTurnAccessor.GetAsync(context, () => new OnTurnProperty("None", new List<EntityProperty>()));

            // Handle case where user is re-introducing themselves.
            // This flow is triggered when we are not in the middle of who-are-you dialog
            // and the user says something like 'call me {username}' or 'my name is {username}'.

            // Get user name entities from on turn property (from the cafe bot dispatcher LUIS model)
            var userNameInOnTurnProperty = (onTurnProperty.Entities ?? new List<EntityProperty>()).Where(item => ((item.EntityName == userNameEntity) || (item.EntityName == userNamePatternAnyEntity)));
            if (userNameInOnTurnProperty.Count() > 0)
            {
                // Get user name from on turn property
                var userName = userNameInOnTurnProperty.First().Value as string;

                // Capitalize user name
                userName = char.ToUpper(userName[0]) + userName.Substring(1);

                // Set user name
                await UserProfileAccessor.SetAsync(context, new UserProfile(userName));

                // End this step so we can greet the user.
                return await stepContext.NextAsync(haveUserName);
            }

            // Prompt user for name if
            // we have an invalid or empty user name or
            // if the user name was previously set to 'Human'
            if (userProfile == null || string.IsNullOrWhiteSpace(userProfile.UserName) || userProfile.UserName.Equals("Human", StringComparison.Ordinal))
            {
                await context.SendActivityAsync("Hello, I'm the Contoso Cafe Bot.");

                // Begin the prompt to ask user their name
                var opts = new PromptOptions
                {
                    Prompt = new Activity
                    {
                        Type = ActivityTypes.Message,
                        Text = "What's your name?",
                    },
                };
                return await stepContext.PromptAsync(askUserNamePrompt, opts);
            }
            else
            {
                // Already have the user name. So just greet them.
                await context.SendActivityAsync($"Hello {userProfile.UserName}, Nice to meet you again! I'm the Contoso Cafe Bot.");

                // End this dialog. We are skipping the next water fall step deliberately.
                return await stepContext.EndDialogAsync();
            }
        }

        /**
          * Waterfall step to finalize user's response and greet user.
          *
          * @param {DialogContext} Dialog context
          * @param {WaterfallStepContext} water fall step context
          */
        private async Task<DialogTurnResult> GreetUserAsync(
                                                WaterfallStepContext stepContext,
                                                CancellationToken cancellationToken)
        {
            var context = stepContext.Context;
            if (stepContext.Result != null)
            {
                var userProfile = await UserProfileAccessor.GetAsync(context);
                await context.SendActivityAsync($"Hey there {userProfile.UserName}!, nice to meet you!");
            }

            return await stepContext.EndDialogAsync();
        }
    }
}
