// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
const { DialogTurnStatus } = require('botbuilder-dialogs');
const { TurnResult } = require('../shared/helpers');

// This dialog's name. Also matches the name of the intent from ../mainDialog/resources/cafeDispatchModel.lu
// LUIS recognizer replaces spaces ' ' with '_'. So intent name 'Who are you' is recognized as 'Who_are_you'.
const FIND_CAFE_LOCATIONS_DIALOG = 'Find_Cafe_Locations';

class FindCafeLocationsDialog {
    constructor() {}
    /**
     * on turn method.
     * @param {Object} context 
     */
    async onTurn(context) {
        await context.sendActivity(`I'm still learning to have a conversation about this topic!`);
        await context.sendActivity(`But, for now, you can find Contoso Cafe locations at https://contosocafe.com`);
        return new TurnResult(DialogTurnStatus.complete);
    }
};

FindCafeLocationsDialog.Name = FIND_CAFE_LOCATIONS_DIALOG;

module.exports = FindCafeLocationsDialog;