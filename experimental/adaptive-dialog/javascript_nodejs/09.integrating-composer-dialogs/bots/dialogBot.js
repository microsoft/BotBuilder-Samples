// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityHandler } = require('botbuilder');
const { DialogManager } = require('botbuilder-dialogs');
const { LanguageGeneratorExtensions, ResourceExtensions } = require('botbuilder-dialogs-adaptive');
const { ResourceExplorer } = require('botbuilder-dialogs-declarative');

class DialogBot extends ActivityHandler {
    /**
     *
     * @param {ConversationState} conversationState
     * @param {UserState} userState
     * @param {Dialog} dialog
     * @param {ResourceExplorer} resourceExplorer
     */
    constructor(conversationState, userState, dialog, resourceExplorer) {
        super();
        if (!conversationState) throw new Error('[DialogBot]: Missing parameter. conversationState is required');
        if (!userState) throw new Error('[DialogBot]: Missing parameter. userState is required');
        if (!dialog) throw new Error('[DialogBot]: Missing parameter. dialog is required');
        if (!resourceExplorer) throw new Error('[DialogBot]: Missing parameter. resourceExplorer is required');

        this.dialogManager = new DialogManager(dialog);
        this.dialogManager.conversationState = conversationState;
        this.dialogManager.userState = userState;
        ResourceExtensions.useResourceExplorer(this.dialogManager, resourceExplorer);
        LanguageGeneratorExtensions.useLanguageGeneration(this.dialogManager);

        this.onTurn(async (context, next) => {
            console.log('Running dialog with activity.');

            await this.dialogManager.onTurn(context);

            await next();
        });
    }
}

module.exports.DialogBot = DialogBot;
