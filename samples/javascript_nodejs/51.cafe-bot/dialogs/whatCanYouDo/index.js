// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
const { Dialog } = require('botbuilder-dialogs');
const { CardFactory } = require('botbuilder');

// Require the adaptive card.
const helpCard = require('./resources/whatCanYouDoCard.json');

// This dialog's name. Also matches the name of the intent from ../dispatcher/resources/cafeDispatchModel.lu
// LUIS recognizer replaces spaces ' ' with '_'. So intent name 'Who are you' is recognized as 'Who_are_you'.
const WHAT_CAN_YOU_DO_DIALOG = 'What_can_you_do';

/**
 *
 * What can you do dialog.
 *   Sends the what can you do adaptive card to user.
 *   Includes a suggested actions of queries users can try. See ../shared/helpers/genSuggestedQueries.js
 *
 */
module.exports = {
    WhatCanYouDoDialog: class extends Dialog {
        static get Name() { return WHAT_CAN_YOU_DO_DIALOG; }
        constructor() {
            super(WHAT_CAN_YOU_DO_DIALOG);
        }
        /**
         * Override dialogBegin.
         *
         * @param {DialogContext} dialog context
         * @param {Object} options
         */
        async dialogBegin(dc, options) {
            await dc.context.sendActivity({ attachments: [CardFactory.adaptiveCard(helpCard)] });
            await dc.context.sendActivity(`Pick a query from the card or you can use the suggestions below.`);
            return await dc.endDialog();
        }
    }
};
