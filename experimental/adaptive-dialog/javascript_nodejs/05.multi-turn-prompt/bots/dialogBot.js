// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityHandler } = require('botbuilder');
const { DialogManager } = require('botbuilder-dialogs');

class DialogBot extends ActivityHandler {
    /**
     *
     * @param {Dialog} dialog
     */
    constructor(conversationState, userState, dialog) {
        super();
        if (!conversationState) throw new Error('[DialogBot]: Missing parameter. conversationState is required');
        if (!userState) throw new Error('[DialogBot]: Missing parameter. userState is required');
        if (!dialog) throw new Error('[DialogBot]: Missing parameter. dialog is required');

        this.dialogManager = new DialogManager(dialog);
        this.dialogManager.conversationState = conversationState;
        this.dialogManager.userState = userState;

        this.onTurn(async (context, next) => {
            console.log('Running dialog with activity.');

            await this.dialogManager.onTurn(context);

            await next();
        });
    }
}

module.exports.DialogBot = DialogBot;
