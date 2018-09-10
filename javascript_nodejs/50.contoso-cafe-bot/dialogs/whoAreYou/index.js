// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const WHO_ARE_YOU = 'Who_are_you';
const { ComponentDialog, DialogTurnStatus, WaterfallDialog, DialogSet, TextPrompt } = require('botbuilder-dialogs');

const DIALOG_START = 'Start';
const USER_NAME = 'userName';
const USER_QUERY = 'userAskedForClarification';
const USER_NO = 'userSaidNoName';
const ASK_USER_NAME_PROMPT = 'askUserName';
const getUserNamePrompt = require('./getUserNamePrompt');
const onTurnProperty = require('../shared/stateProperties/onTurnProperty');
const turnResult = require('../shared/turnResult');
class WhoAreYouDialog extends ComponentDialog {
    constructor(userProfilePropertyAccessor, botConfig, turnCounterPropertyAccessor, onTurnPropertyAccessor) {
        super(WHO_ARE_YOU);
        if(!userProfilePropertyAccessor) throw ('Need user profile property accessor');
        if(!botConfig) throw ('Need bot config');
        this.userProfilePropertyAccessor = userProfilePropertyAccessor;
        this.botConfig = botConfig;
        this.onTurnPropertyAccessor = onTurnPropertyAccessor;
        // add dialogs
        this.addDialog(new WaterfallDialog(DIALOG_START, [
            this.askForUserName,
            this.greetUser
        ]));
        this.addDialog(new getUserNamePrompt(ASK_USER_NAME_PROMPT, botConfig, userProfilePropertyAccessor, turnCounterPropertyAccessor, onTurnPropertyAccessor));
    }

    async askForUserName(dc, step) {
        return await dc.prompt(ASK_USER_NAME_PROMPT, `What's your name?`);
    }

    async greetUser(dc, step) {
        if(step.result.reason && step.result.reason === 'Interruption') {
            // set onTurnProperty in the payload so this can be resumed back if needed by main dialog.
            if(step.result.payload === undefined) {
                step.result.payload = {onTurnProperty: new onTurnProperty(WHO_ARE_YOU)};
            }
            else {
                step.result.payload.onTurnProperty = new onTurnProperty(WHO_ARE_YOU);
            }
            return new turnResult(DialogTurnStatus.empty, step.result);
        }
        return await dc.end();
    }
};

WhoAreYouDialog.Name = WHO_ARE_YOU;

module.exports = WhoAreYouDialog;