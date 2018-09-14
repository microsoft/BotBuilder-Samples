// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
const { ComponentDialog, DialogTurnStatus, WaterfallDialog } = require('botbuilder-dialogs');
const { OnTurnProperty } = require('../shared/stateProperties');
const { GetUserNamePrompt } = require('../shared/prompts');
const { TurnResultHelper } = require('../shared/helpers');
const { InterruptionDispatcher } = require('../interruptionDispatcher');

// This dialog's name. Also matches the name of the intent from ../mainDispatcher/resources/cafeDispatchModel.lu
// LUIS recognizer replaces spaces ' ' with '_'. So intent name 'Who are you' is recognized as 'Who_are_you'.
const WHO_ARE_YOU_DIALOG = 'Who_are_you';
const ASK_USER_NAME_PROMPT = 'askUserNamePrompt';
const DIALOG_START = 'Who_are_you_start';

/**
 * Class Who are you dialog.
 */
module.exports = {
    WhoAreYouDialog: class extends ComponentDialog {
        static get Name() { return WHO_ARE_YOU_DIALOG; }
        /**
         * Constructor.
         * 
         * @param {Object} botConfig bot configuration
         * @param {Object} userProfilePropertyAccessor property accessor for user profile property
         * @param {Object} conversationState 
         */
        constructor(botConfig, userProfilePropertyAccessor, onTurnPropertyAccessor, conversationState) {
            super (WHO_ARE_YOU_DIALOG);
            if (!botConfig) throw ('Missing parameter. Bot configuration is required.');
            if (!userProfilePropertyAccessor) throw ('Missing parameter. User profile property accessor is required.');
            if (!conversationState) throw ('Missing parameter. Conversation state is required.');

            this.onTurnPropertyAccessor = onTurnPropertyAccessor;

            // add dialogs
            this.addDialog(new WaterfallDialog(DIALOG_START, [
                this.askForUserName.bind(this),
                this.greetUser.bind(this)
            ]));

            // add prompts
            this.addDialog(new GetUserNamePrompt(ASK_USER_NAME_PROMPT, 
                                                botConfig, 
                                                userProfilePropertyAccessor, 
                                                conversationState));

            // This dialog is interruptable. So add interruptionDispatcherDialog
            this.addDialog(new InterruptionDispatcher(onTurnPropertyAccessor, conversationState, userProfilePropertyAccessor));
        }
        /**
         * Waterfall step to prompt for user's name
         * 
         * @param {Object} dc Dialog context
         * @param {Object} step Dialog turn result
         */
        async askForUserName(dc, step) {
            return await dc.prompt(ASK_USER_NAME_PROMPT, `What's your name?`);
        }
        /**
         * Waterfall step to finalize user's response and greet user.
         * 
         * @param {Object} dc Dialog context
         * @param {Object} step Dialog turn result
         */
        async greetUser(dc, step) {
            // Handle interruption.
            if (step.result.reason && step.result.reason === 'Interruption') {
                // Get on turn properties
                const onTurnProperty = await this.onTurnPropertyAccessor.get(dc.context);
                await dc.begin(InterruptionDispatcher.Name, onTurnProperty);
                // // Set onTurnProperty in the payload so this can be resumed back if needed by main dialog.
                // if (step.result.payload === undefined) {
                //     step.result.payload = {onTurnProperty: new OnTurnProperty(WHO_ARE_YOU_DIALOG)};
                // }
                // else {
                //     step.result.payload.onTurnProperty = new OnTurnProperty(WHO_ARE_YOU_DIALOG);
                // }
                // return new TurnResultHelper(DialogTurnStatus.empty, step.result);
            }
            return await dc.end();
        }
    }
};