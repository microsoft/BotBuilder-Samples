// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityHandler } = require('botbuilder');

class DialogBot extends ActivityHandler {
    /**
     * 
     * @param {ConversationState} conversationState 
     * @param {UserState} userState 
     * @param {Dialog} dialog 
     */
    constructor(conversationState, userState, dialog) {
        super();

        if (!conversationState) { throw new Error("[DialogBot]: Missing conversationState parameter."); }
        if (!userState) { throw new Error("[DialogBot]: Missing userState parameter."); }
        if (!dialog) { throw new Error("[DialogBot]: Missing dialog parameter."); }

        this.conversationState = conversationState;
        this.userState = userState;
        this.dialog = dialog;

        this.onTurn(async (turnContext, next) => {
            await this.conversationState.saveChanges(turnContext, false);
            await this.userState.saveChanges(turnContext, false);

            await next();
        });

        this.onMessage(async (turnContext, next) => {
            console.log('Running dialog with Message Activity.');
            let dialogState = this.conversationState.createProperty('');
            
            await this.dialog.run(turnContext, dialogState);

            await next();
        });
    }
}

module.exports.DialogBot = DialogBot;