// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
const { ComponentDialog, DialogTurnStatus, WaterfallDialog } = require('botbuilder-dialogs');

const getUserNamePrompt = require('../shared/prompts/getUserNamePrompt');
const onTurnProperty = require('../shared/stateProperties/onTurnProperty');
const turnResult = require('../shared/turnResult');

// This dialog's name. Also matches the name of the intent from ../mainDialog/resources/cafeDispatchModel.lu
// LUIS recognizer replaces spaces ' ' with '_'. So intent name 'Who are you' is recognized as 'Who_are_you'.
const WHO_ARE_YOU = 'Who_are_you';

const DIALOG_START = 'Start';
const ASK_USER_NAME_PROMPT = 'askUserName';

/**
 * Class Who are you dialog.
 */
class WhoAreYouDialog extends ComponentDialog {
    /**
     * Constructor.
     * 
     * @param {Object} botConfig bot configuration
     * @param {Object} userProfilePropertyAccessor property accessor for user profile property
     * @param {Object} turnCounterPropertyAccessor property accessor for turn counter property
     */
    constructor(botConfig, userProfilePropertyAccessor, turnCounterPropertyAccessor) {
        super(WHO_ARE_YOU);
        if(!botConfig) throw ('Need bot config');
        if(!userProfilePropertyAccessor) throw ('Need user profile property accessor');
        if(!turnCounterPropertyAccessor) throw ('Need turn counter property accessor');

        // add dialogs
        this.addDialog(new WaterfallDialog(DIALOG_START, [
            this.askForUserName,
            this.greetUser
        ]));
        this.addDialog(new getUserNamePrompt(ASK_USER_NAME_PROMPT, botConfig, userProfilePropertyAccessor, turnCounterPropertyAccessor));
    }
    /**
     * Waterfall step to prompt for user's name
     * 
     * @param {Object} dc Dialog context
     * @param {Object} step Dialog turn result
     */
    async askForUserName(dc, step) {
        return await dc.prompt(ASK_USER_NAME_PROMPT, `What's your name?`);
    }
    /**
     * Waterfall step to finalize user's response and greet user.
     * 
     * @param {Object} dc Dialog context
     * @param {Object} step Dialog turn result
     */
    async greetUser(dc, step) {
        // Handle interruption.
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