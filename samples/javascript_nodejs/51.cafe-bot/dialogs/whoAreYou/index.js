// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
const { ComponentDialog, ConfirmPrompt, WaterfallDialog } = require('botbuilder-dialogs');
const { GetUserNamePrompt } = require('../shared/prompts');
const { InterruptionDispatcher } = require('../dispatcher/interruptionDispatcher');
const { UserProfile } = require('../shared/stateProperties');
const { LUIS_INTENTS, LUIS_ENTITIES } = require('../shared/helpers');

// This dialog's name. Matches the name of the LUIS intent from ../dispatcher/resources/cafeDispatchModel.lu
const WHO_ARE_YOU_DIALOG = LUIS_INTENTS.Who_are_you;

// User name entity from ../whoAreYou/resources/whoAreYou.lu
const USER_NAME_ENTITY = LUIS_ENTITIES.user_name_simple;
const USER_NAME_PATTERN_ANY_ENTITY = LUIS_ENTITIES.user_name_patternAny;

// Names for dialogs and prompts
const ASK_USER_NAME_PROMPT = 'askUserNamePrompt';
const DIALOG_START = 'Who_are_you_start';
const CONFIRM_CANCEL_PROMPT = 'confirmCancelPrompt';

const HAVE_USER_NAME = true;
/**
 * Class Who are you dialog.
 *
 *  Uses a waterfall dialog with a custom text prompt to get user's name and greet them.
 *  Has two water fall steps -
 *    1. Skips to water fall step #2 if we already have user's name or the main dispatcher picked up their name.
 *       Initiates the 'ask user name' prompt if we dont have their name yet
 *    2. Greets user with their name
 *
 * 'Ask user name' prompt is implemented as a custom text prompt. See ../shared/prompts/getUserNamePrompt.js
 *
 */
module.exports = {
    WhoAreYouDialog: class extends ComponentDialog {
        static get Name() { return WHO_ARE_YOU_DIALOG; }
        /**
         * Constructor.
         *
         * @param {BotConfiguration} bot configuration
         * @param {ConversationState} conversationState
         * @param {StatePropertyAccessor} accessor for user profile property
         * @param {StatePropertyAccessor} accessor for on turn property
         * @param {StatePropertyAccessor} accessor for reservation property
         */
        constructor(botConfig, conversationState, userProfileAccessor, onTurnAccessor, reservationAccessor) {
            super(WHO_ARE_YOU_DIALOG);

            if (!botConfig) throw new Error('Missing parameter. Bot configuration is required.');
            if (!userProfileAccessor) throw new Error('Missing parameter. User profile property accessor is required.');
            if (!conversationState) throw new Error('Missing parameter. Conversation state is required.');

            // Keep accessors for the steps to consume.
            this.onTurnAccessor = onTurnAccessor;
            this.userProfileAccessor = userProfileAccessor;

            // Add water fall dialog with two steps.
            this.addDialog(new WaterfallDialog(DIALOG_START, [
                this.askForUserName.bind(this),
                this.greetUser.bind(this)
            ]));

            // Add get user name prompt
            this.addDialog(new GetUserNamePrompt(ASK_USER_NAME_PROMPT,
                botConfig,
                userProfileAccessor,
                conversationState,
                onTurnAccessor));

            // This dialog is multi-turn capable and also interruptable. Add interruptionDispatcherDialog
            this.addDialog(new InterruptionDispatcher(onTurnAccessor, conversationState, userProfileAccessor, botConfig, reservationAccessor));

            // When user decides to abandon this dialog, we need to confirm user action. Add confirmation prompt.
            this.addDialog(new ConfirmPrompt(CONFIRM_CANCEL_PROMPT));
        }
        /**
         * Waterfall step to prompt for user's name
         *
         * @param {WaterfallStepContext} water fall step context
         */
        async askForUserName(step) {
            // Get user profile.
            let userProfile = await this.userProfileAccessor.get(step.context);
            // Get on turn properties.
            const onTurnProperty = await this.onTurnAccessor.get(step.context);

            // Handle case where user is re-introducing themselves.
            // This flow is triggered when we are not in the middle of who-are-you dialog
            //   and the user says something like 'call me {username}' or 'my name is {username}'.

            // Get user name entities from on turn property (from the cafe bot dispatcher LUIS model)
            let userNameInOnTurnProperty = (onTurnProperty.entities || []).filter(item => (
                (item.entityName === USER_NAME_ENTITY) ||
                (item.entityName === USER_NAME_PATTERN_ANY_ENTITY)));
            if (userNameInOnTurnProperty.length !== 0) {
                // get user name from on turn property
                let userName = userNameInOnTurnProperty[0].entityValue[0];
                // capitalize user name
                userName = userName.charAt(0).toUpperCase() + userName.slice(1);
                // set user name
                await this.userProfileAccessor.set(step.context, new UserProfile(userName));
                // End this step so we can greet the user.
                return await step.next(HAVE_USER_NAME);
            }

            // Prompt user for name if
            //    we have an invalid or empty user name or
            //    if the user name was previously set to 'Human'

            if (userProfile === undefined || userProfile.userName === '' || userProfile.userName === 'Human') {
                await step.context.sendActivity(`Hello, I'm the Contoso Cafe Bot.`);
                // Begin the prompt to ask user their name
                return await step.prompt(ASK_USER_NAME_PROMPT, `What's your name?`);
            } else {
                // Already have the user name. So just greet them.
                await step.context.sendActivity(`Hello ${ userProfile.userName }, Nice to meet you again! I'm the Contoso Cafe Bot.`);

                // End this dialog. We are skipping the next water fall step deliberately.
                return await step.endDialog();
            }
        }
        /**
         * Waterfall step to finalize user's response and greet user.
         *
         * @param {WaterfallStepContext} water fall step context
         */
        async greetUser(step) {
            if (step.result) {
                const userProfile = await this.userProfileAccessor.get(step.context);
                await step.context.sendActivity(`Hey there ${ userProfile.userName }!, nice to meet you!`);
            }
            return await step.endDialog();
        }
    }
};
