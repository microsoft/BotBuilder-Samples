// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
const { ComponentDialog } = require('botbuilder-dialogs');
const { QnADialog } = require('../qna');
const { ChitChatDialog } = require('../chitChat');
const { HelpDialog } = require('../help');
const { WhatCanYouDoDialog } = require('../whatCanYouDo');

const NONE_INTENT = 'None';
const WHO_ARE_YOU_DIALOG_NAME = 'Who_are_you';
const BOOK_TABLE_DIALOG_NAME = 'Book_Table';
const INTERRUPTION_DISPATCHER_DIALOG = 'interruptionDispatcherDialog';

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
        }
        /**
         * Override onDialogBegin
         *
         * @param {Object} dc dialog context
         * @param {Object} options dialog turn options
         */
        async onDialogBegin(dc, options) {
            // Override default begin() logic with interruption orchestration logic
            return await this.interruptionDispatch(dc, options);
        }
        /**
         * Override onDialogContinue
         *
         * @param {Object} dc dialog context
         */
        async onDialogContinue(dc) {
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
            // See if interruption is allowed
            if (options === undefined || options.intent === undefined) {
                await dc.context.sendActivity(`Sorry. I'm unable to do that right now. You can cancel the current conversation and start a new one`);
                return await dc.endDialog();
            }
            switch (options.intent) {
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
