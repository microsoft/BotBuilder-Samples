// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
const { ComponentDialog } = require('botbuilder-dialogs');
const { QnADialog } = require('../qna');
const { ChitChatDialog } = require('../chitChat');
const { HelpDialog } = require('../help');
const { WhatCanYouDoDialog } = require('../whatCanYouDo');
const { LuisRecognizer } = require('botbuilder-ai');
const { OnTurnProperty } = require('../shared/stateProperties');

const NONE_INTENT = 'None';
const WHO_ARE_YOU_DIALOG_NAME = 'Who_are_you';
const BOOK_TABLE_DIALOG_NAME = 'Book_Table';
const INTERRUPTION_DISPATCHER_DIALOG = 'interruptionDispatcherDialog';

// LUIS service type entry in the .bot file for dispatch.
const LUIS_CONFIGURATION = 'cafeDispatchModel';

module.exports = {
    InterruptionDispatcher: class extends ComponentDialog {
        static get Name() { return INTERRUPTION_DISPATCHER_DIALOG; }

        /**
         * Constructor.
         *
         * @param {StatePropertyAccessor} onTurnAccessor
         * @param {ConversationState} conversationState
         * @param {StatePropertyAccessor} userProfileAccessor
         * @param {BotConfiguration} botConfig
         */
        constructor(onTurnAccessor, conversationState, userProfileAccessor, botConfig) {
            super(INTERRUPTION_DISPATCHER_DIALOG);

            if (!onTurnAccessor) throw new Error('Missing parameter. On turn property accessor is required.');
            if (!conversationState) throw new Error('Missing parameter. Conversation state is required.');
            if (!userProfileAccessor) throw new Error('Missing parameter. User profile accessor is required.');
            if (!botConfig) throw new Error('Missing parameter. Bot configuration is required.');

            // keep on turn accessor
            this.onTurnAccessor = onTurnAccessor;

            // add dialogs
            this.addDialog(new WhatCanYouDoDialog());
            this.addDialog(new QnADialog(botConfig, userProfileAccessor));

            // add recognizer
            const luisConfig = botConfig.findServiceByNameOrId(LUIS_CONFIGURATION);
            if (!luisConfig || !luisConfig.appId) throw new Error(`Cafe Dispatch LUIS model not found in .bot file. Please ensure you have all required LUIS models created and available in the .bot file. See readme.md for additional information.\n`);
            this.luisRecognizer = new LuisRecognizer({
                applicationId: luisConfig.appId,
                endpoint: luisConfig.getEndpoint(),
                // CAUTION: Authoring key is used in this example as it is appropriate for prototyping.
                // When implimenting for deployment/production, assign and use a subscription key instead of an authoring key.
                endpointKey: luisConfig.authoringKey
            });
        }
        /**
         * Override onBeginDialog
         *
         * @param {Object} dc dialog context
         * @param {Object} options dialog turn options
         */
        async onBeginDialog(dc, options) {
            // Override default begin() logic with interruption orchestration logic
            return await this.interruptionDispatch(dc, options);
        }
        /**
         * Override onContinueDialog
         *
         * @param {Object} dc dialog context
         */
        async onContinueDialog(dc) {
            // Override default continue() logic with interruption orchestration logic
            return await this.interruptionDispatch(dc);
        }
        /**
         * Helper method to dispatch on interruption.
         *
         * @param {Object} dc
         * @param {Object} options
         */
        async interruptionDispatch(dc, options) {
            // Call to LUIS recognizer to get intent + entities
            const LUISResults = await this.luisRecognizer.recognize(dc.context);

            // Return new instance of on turn property from LUIS results.
            // Leverages static fromLUISResults method
            let onTurnProperty = OnTurnProperty.fromLUISResults(LUISResults);

            switch (onTurnProperty.intent) {
            // Help, ChitChat and QnA share the same QnA Maker model. So just call the QnA Dialog.
            case QnADialog.Name:
            case ChitChatDialog.Name:
            case HelpDialog.Name:
                return await dc.beginDialog(QnADialog.Name);
            case WhatCanYouDoDialog.Name:
            case WHO_ARE_YOU_DIALOG_NAME:
            case BOOK_TABLE_DIALOG_NAME:
                await dc.context.sendActivity(`Sorry. I'm unable to do that right now. You can cancel the current conversation and start a new one`);
                return await dc.endDialog();
            case NONE_INTENT:
            default:
                await dc.context.sendActivity(`I'm still learning.. Sorry, I do not know how to help you with that.`);
                return await dc.context.sendActivity(`Follow [this link](https://www.bing.com/search?q=${ dc.context.activity.text }) to search the web!`);
            }
        }
    }
};
