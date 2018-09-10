// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const WHAT_CAN_YOU_DO = 'What_can_you_do';
const turnResult = require('../shared/turnResult');
const { DialogTurnStatus } = require('botbuilder-dialogs');
const helpCard = require('./resources/whatCanYouDoCard.json');
const { ActivityTypes, CardFactory, MessageFactory } = require('botbuilder');
class WhatCanYouDoDialog {
    constructor() {

    }
    async onTurn(context) {
        await context.sendActivity({ attachments: [CardFactory.adaptiveCard(helpCard)]});
        return new turnResult(DialogTurnStatus.complete);
    }
};

WhatCanYouDoDialog.Name = WHAT_CAN_YOU_DO;

module.exports = WhatCanYouDoDialog;