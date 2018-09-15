// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
const { Dialog } = require('botbuilder-dialogs');
const { CardFactory, MessageFactory } = require('botbuilder');
const { GenSuggestedQueries } = require('../shared/helpers/genSuggestedQueries');

// Require the adaptive card.
const helpCard = require('./resources/whatCanYouDoCard.json');

// This dialog's name. Also matches the name of the intent from ../mainDispatcher/resources/cafeDispatchModel.lu
// LUIS recognizer replaces spaces ' ' with '_'. So intent name 'Who are you' is recognized as 'Who_are_you'.
const WHAT_CAN_YOU_DO_DIALOG = 'What_can_you_do';

/**
 * Class What can you do dialog.
 */
module.exports = {
    WhatCanYouDoDialog: class extends Dialog {
        static get Name() { return WHAT_CAN_YOU_DO_DIALOG; }
        constructor() {
            super (WHAT_CAN_YOU_DO_DIALOG);
        }
        /**
         * Override dialogBegin. 
         * 
         * @param {Object} dc dialog context
         * @param {Object} options options
         */
        async dialogBegin(dc, options) {
            await dc.context.sendActivity({ attachments: [CardFactory.adaptiveCard(helpCard)]});
            await dc.context.sendActivity(MessageFactory.suggestedActions(GenSuggestedQueries(), `Pick a query from the card or you can use the suggestions below.`));
            return await dc.end();
        }
    }
};