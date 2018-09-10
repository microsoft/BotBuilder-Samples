// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const FIND_CAFE_LOCATIONS = 'Find_Cafe_Locations';
const turnResult = require('../shared/turnResult');
const { DialogTurnStatus } = require('botbuilder-dialogs');
class FindCafeLocationsDialog {
    constructor() {

    }
    async onTurn(context) {
        await context.sendActivity(`I'm still learning to have a conversation about this topic!`);
        await context.sendActivity(`But, for now, you can find Contoso Cafe locations at https://contosocafe.com`);
        return new turnResult(DialogTurnStatus.complete);
    }
};

FindCafeLocationsDialog.Name = FIND_CAFE_LOCATIONS;

module.exports = FindCafeLocationsDialog;