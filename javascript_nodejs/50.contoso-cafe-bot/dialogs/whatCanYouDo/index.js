// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
const { DialogTurnStatus, Dialog } = require('botbuilder-dialogs');
const { CardFactory, MessageFactory } = require('botbuilder');
const { TurnResult } = require('../shared/helpers');
const { GenSuggestedQueries } = require('../shared/helpers/genSuggestedQueries');

// Require the adaptive card.
const helpCard = require('./resources/whatCanYouDoCard.json');

// This dialog's name. Also matches the name of the intent from ../mainDialog/resources/cafeDispatchModel.lu
// LUIS recognizer replaces spaces ' ' with '_'. So intent name 'Who are you' is recognized as 'Who_are_you'.
const WHAT_CAN_YOU_DO_DIALOG = 'What_can_you_do';

/**
 * Class What can you do dialog.
 */
class WhatCanYouDoDialog extends Dialog {
    constructor() {}
    /**
     * On turn method.
     * 
     * @param {Object} context context object
     */
    async onTurn(context) {
        await context.sendActivity({ attachments: [CardFactory.adaptiveCard(helpCard)]});
        await context.sendActivity(MessageFactory.suggestedActions(GenSuggestedQueries(), `Pick a query from the card or you can use the suggestions below.`));
        return new TurnResult(DialogTurnStatus.complete);
    }
};

WhatCanYouDoDialog.Name = WHAT_CAN_YOU_DO_DIALOG;

module.exports = WhatCanYouDoDialog;