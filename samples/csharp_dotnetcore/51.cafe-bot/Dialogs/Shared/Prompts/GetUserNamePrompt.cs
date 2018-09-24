// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;

namespace Microsoft.BotBuilderSamples
{
    public class GetUserNamePrompt : TextPrompt
    {
        private const string InterruptionDispatcher = "interruptionDispatcherDialog";
        // LUIS service type entry for the get user profile LUIS model in the .bot file.
        private const string LUIS_CONFIGURATION = "getUserProfile";
        // LUIS intent names from ./resources/getUserProfile.lu
        private const string WHY_DO_YOU_ASK_INTENT = "Why_do_you_ask";
        private const string GET_USER_NAME_INTENT = "Get_user_name";
        private const string NO_NAME_INTENT = "No_Name";
        private const string NONE_INTENT = "None";
        private const string CANCEL_INTENT = "Cancel";
        // User name entity from ./resources/getUserProfile.lu
        private const string USER_NAME = "userName";
        private const string USER_NAME_PATTERN_ANY = "userName_patternAny";
        private const string TURN_COUNTER_PROPERTY = "turnCounterProperty";
        private const bool HAVE_USER_PROFILE = true;
        private const bool NO_USER_PROFILE = false;
        private const string CONFIRM_CANCEL_PROMPT = "confirmCancelPrompt";

        private readonly BotServices _botServices;
        private readonly IStatePropertyAccessor<UserProfile> _userProfileAccessor;
        private readonly ConversationState _conversationState;
        private readonly IStatePropertyAccessor<OnTurnProperty> _onTurnAccessor;
        protected readonly IStatePropertyAccessor<int> _turnCounterAccessor;

        /**
         * Constructor.
         * This is a custom TextPrompt that uses a LUIS model to handle turn.n conversations including interruptions.
         * @param {String} dialogId Dialog ID
         * @param {BotConfiguration} botConfig Bot configuration
         * @param {StatePropertyAccessor} userProfileAccessor accessor for user profile property
         * @param {ConversationState} conversationState conversations state
         */
        public GetUserNamePrompt(
                    string dialogId,
                    BotServices botServices,
                    IStatePropertyAccessor<UserProfile> userProfileAccessor,
                    ConversationState conversationState,
                    IStatePropertyAccessor<OnTurnProperty> onTurnAccessor,
                    IStatePropertyAccessor<int> onTurnCounterAccessor,
                    PromptValidator<string> validator = null
            )
            : base(dialogId, validator)
        {
            _botServices = botServices ?? throw new ArgumentNullException(nameof(botServices));
            _userProfileAccessor = userProfileAccessor ?? throw new ArgumentNullException(nameof(botServices));
            _conversationState = conversationState ?? throw new ArgumentNullException(nameof(conversationState));
            _onTurnAccessor = onTurnAccessor ?? throw new ArgumentNullException(nameof(onTurnAccessor));
            _turnCounterAccessor = onTurnCounterAccessor ?? throw new ArgumentNullException(nameof(_turnCounterAccessor));
        }

        /**
         * Override continueDialog.
         *   The override enables
         *     Interruption to be kicked off from right within this dialog.
         *     Ability to leverage a dedicated LUIS model to provide flexible entity filling,
         *     corrections and contextual help.
         *
         * @param {DialogContext} dialog context
         */
        public async override Task<DialogTurnResult> ContinueDialogAsync(DialogContext dc, CancellationToken cancellationToken = default(CancellationToken))
        {
            var context = dc.Context;

            // Get turn counter
            var turnCounter = await _turnCounterAccessor.GetAsync(context, () => 0);
            turnCounter = ++turnCounter;
            // set updated turn counter
            await _turnCounterAccessor.SetAsync(context, turnCounter);
            // See if we have card input. This would come in through onTurnProperty
            var onTurnProperty = await _onTurnAccessor.GetAsync(context);
            if (onTurnProperty != null)
            {
                if (onTurnProperty.Entities.Count > 0)
                {
                    // find user name in on turn property
                    var userNameInOnTurnProperty = onTurnProperty.Entities.FirstOrDefault(item => string.Compare(item.EntityName, USER_NAME, StringComparison.Ordinal) == 0);
                    if (userNameInOnTurnProperty != null)
                    {
                        var userName = userNameInOnTurnProperty.Value as string;
                        if (!string.IsNullOrWhiteSpace(userName))
                        {
                            await UpdateUserProfileProperty(userName, context);
                        }

                        return await ContinueDialogAsync(dc);
                    }
                }
            }

            if (turnCounter >= 1)
            {
                // We we need to get user's name right. Include a card.
                await context.SendActivityAsync(
                    new Activity
                    {
                        Attachments = new List<Attachment>
                        {
                            Helpers.CreateAdaptiveCardAttachment(@"..\..\WhoAreYou\Resources\getUserNameCard.json"),
                        },
                    });
            }
            else if (turnCounter >= 3)
            {
                // We are not going to spend more than 3 turns to get user's name.
                return await EndGetUserNamePrompt(dc);
            }

            // call LUIS and get results
            var LUISResults = await _botServices.LuisServices[LUIS_CONFIGURATION].RecognizeAsync(context, cancellationToken);

            var topLuisIntent = LUISResults.GetTopScoringIntent();
            var topIntent = topLuisIntent.intent;

            if (string.IsNullOrWhiteSpace(topIntent))
            {
                // go with intent in onTurnProperty
                topIntent = string.IsNullOrWhiteSpace(onTurnProperty.Intent) ? "None" : onTurnProperty.Intent;
            }
            // Did user ask for help or said they are not going to give us the name?
            switch (topIntent)
            {
                case NO_NAME_INTENT:
                    // set user name in profile to Human
                    await _userProfileAccessor.SetAsync(context, new UserProfile("Human"));
                    return await EndGetUserNamePrompt(dc);
                case GET_USER_NAME_INTENT:
                    // Find the user's name from LUIS entities list.
                    if (LUISResults.Entities.TryGetValue(USER_NAME, out var entity))
                    {
                        var userName = (string)entity[USER_NAME][0];
                        await UpdateUserProfileProperty(userName, context);
                        return await ContinueDialogAsync(dc);
                    }
                    else if (LUISResults.Entities.TryGetValue(USER_NAME_PATTERN_ANY, out var entity_pattery))
                    {
                        var userName = (string)entity_pattery[USER_NAME_PATTERN_ANY][0];
                        await UpdateUserProfileProperty(userName, context);
                        return await ContinueDialogAsync(dc);
                    }
                    else
                    {
                        await context.SendActivityAsync("Sorry, I didn't get that. What's your name ?");
                        return await ContinueDialogAsync(dc);
                    }

                case WHY_DO_YOU_ASK_INTENT:
                    await context.SendActivityAsync("I need your name to be able to address you correctly!");
                    await context.SendActivityAsync(MessageFactory.SuggestedActions(new List<string> { "I won't give you my name", "What is your name?" }));
                    return await ContinueDialogAsync(dc);

                case NONE_INTENT:
                    await UpdateUserProfileProperty(context.Activity.Text, context);
                    return await ContinueDialogAsync(dc);

                case CANCEL_INTENT:
                    // start confirmation prompt
                    var opts = new PromptOptions
                    {
                        Prompt = new Activity
                        {
                            Type = ActivityTypes.Message,
                            Text = "Are you sure you want to cancel ?",
                        },
                    };
                    return await dc.PromptAsync(CONFIRM_CANCEL_PROMPT, opts);

                default:
                    // Handle interruption.
                    var onTurnPropertyValue = await _onTurnAccessor.GetAsync(dc.Context);
                    return await dc.BeginDialogAsync(InterruptionDispatcher, onTurnPropertyValue);
            }
        }

        /**
         * Helper method to end this prompt
         *
         * @param {DialogContext} innerDc
         */
        protected async Task<DialogTurnResult> EndGetUserNamePrompt(DialogContext innerDc)
        {
            var context = innerDc.Context;
            await context.SendActivityAsync("No worries. Hello Human, nice to meet you!");
            await context.SendActivityAsync("You can always say 'My name is <your name>' to introduce yourself to me.");

            // End this dialog since user does not wish to proceed further.
            return await innerDc.EndDialogAsync(NO_USER_PROFILE);
        }

        /**
         * Override resumeDialog. This is used to handle user's response to confirm cancel prompt.
         *
         * @param {DialogContext} dc
         * @param {DialogReason} reason
         * @param {Object} result
         */
        public async override Task<DialogTurnResult> ResumeDialogAsync(DialogContext dc, DialogReason reason, object result = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (result == null)
            {
                // User said yes to cancel prompt.
                await dc.Context.SendActivityAsync("Sure. I've cancelled that!");
                return await dc.CancelAllDialogsAsync();
            }
            else
            {
                // User said no to cancel.
                return await base.ResumeDialogAsync(dc, reason, result);
            }
        }

        /**
         * Helper method to set user name
         *
         * @param {String} userName
         * @param {TurnContext} context
         */
        protected async Task UpdateUserProfileProperty(string userName, ITurnContext context)
        {
            var userProfile = await _userProfileAccessor.GetAsync(context);
            if (userProfile == null)
            {
                userProfile = new UserProfile(userName);
            }
            else
            {
                userProfile.UserName = userName;
            }

            await _userProfileAccessor.SetAsync(context, userProfile);
        }

        // Create an attachment message response.
        private Activity CreateResponse(Activity activity, Attachment attachment)
        {
            var response = activity.CreateReply();
            response.Attachments = new List<Attachment>() { attachment };
            return response;
        }

        // Load attachment from file
        private Attachment CreateAdaptiveCardAttachment(string fullPath)
        {
            var adaptiveCard = File.ReadAllText(fullPath);
            return new Attachment()
            {
                ContentType = "application/vnd.microsoft.card.adaptive",
                Content = JsonConvert.DeserializeObject(adaptiveCard),
            };
        }

    }
}
