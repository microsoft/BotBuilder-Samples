// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
const { ComponentDialog, DialogSet } = require('botbuilder-dialogs');
const { QnADialog } = require('../qna');
const { ChitChatDialog } = require('../chitChat');
const { HelpDialog } = require('../help');
const { CancelDialog } = require('../cancel');
const { WhatCanYouDoDialog } = require('../whatCanYouDo');
const { FindCafeLocationsDialog } = require('../findCafeLocations');
const { BookTableDialog } = require('../bookTable/index');

const NONE_INTENT = 'None';
const INTERRUPTION_DISPATCHER_DIALOG = 'interruptionDispatcherDialog';

module.exports = {
    InterruptionDispatcher: class extends ComponentDialog {
        static get Name() { return INTERRUPTION_DISPATCHER_DIALOG; }

        /**
         * Constructor. 
         * 
         * @param {Object} onTurnAccessor 
         * @param {Object} conversationState 
         * @param {Object} userProfileAccessor 
         * @param {Object} botConfig 
         */
        constructor(onTurnAccessor, conversationState, userProfileAccessor, botConfig, reservationsAccessor) {
            super (INTERRUPTION_DISPATCHER_DIALOG);
            
            if (!onTurnAccessor) throw ('Missing parameter. On turn property accessor is required.');
            if (!conversationState) throw ('Missing parameter. Conversation state is required.');
            if (!userProfileAccessor) throw ('Missing parameter. User profile accessor is required.');
            if (!botConfig) throw ('Missing parameter. Bot configuration is required.');

            // keep on turn accessor
            this.onTurnAccessor = onTurnAccessor;

            // add dialogs
            this.addDialog(new WhatCanYouDoDialog());
            this.addDialog(new FindCafeLocationsDialog());
            this.addDialog(new QnADialog(botConfig, userProfileAccessor));
            this.addDialog(new BookTableDialog(botConfig, reservationsAccessor, onTurnAccessor, userProfileAccessor, conversationState));
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
            if(options == undefined || options.intent === undefined) {
                await dc.context.sendActivity(`Sorry. I'm unable to do that right now. You can cancel the current conversation and start a new one`);
                return await dc.end();
            }
            switch(options.intent) {
                // Help, ChitChat and QnA share the same QnA Maker model. So just call the QnA Dialog.
                case QnADialog.Name: 
                case ChitChatDialog.Name: 
                case HelpDialog.Name: 
                    return await dc.begin(QnADialog.Name);
                case CancelDialog.Name: 
                case WhatCanYouDoDialog.Name:
                case BookTableDialog.Name:
                    await dc.context.sendActivity(`Sorry. I'm unable to do that right now. You can cancel the current conversation and start a new one`);
                    return await dc.end();
                case FindCafeLocationsDialog.Name:
                    return await dc.begin(FindCafeLocationsDialog.Name);
                case NONE_INTENT:
                default:
                    await dc.context.sendActivity(`I'm still learning.. Sorry, I do not know how to help you with that.`);
                    return await dc.context.sendActivity(`Follow [this link](https://www.bing.com/search?q=${dc.context.activity.text}) to search the web!`);
            }
        }
    }
};