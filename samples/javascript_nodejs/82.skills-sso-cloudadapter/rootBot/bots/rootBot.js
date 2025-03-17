// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// @ts-check

const { ActivityHandler, ActivityTypes, MessageFactory } = require('botbuilder');
const { runDialog } = require('botbuilder-dialogs');

/** @import { ConversationState } from 'botbuilder' */
/** @import { MainDialog } from '../dialogs/mainDialog' */

class RootBot extends ActivityHandler {
    /**
     *
     * @param {ConversationState} conversationState
     * @param {MainDialog} dialog
     */
    constructor(conversationState, dialog) {
        super();
        if (!conversationState) throw new Error('[RootBot]: Missing parameter. conversationState is required');
        if (!dialog) throw new Error('[RootBot]: Missing parameter. dialog is required');

        this.conversationState = conversationState;
        this.dialog = dialog;

        this.onTurn(async (turnContext, next) => {
            if (turnContext.activity.type !== ActivityTypes.ConversationUpdate) {
                // Run the Dialog with the activity.
                await runDialog(this.dialog, turnContext, this.conversationState.createProperty('DialogState'));
            }

            await next();
        });

        this.onMembersAdded(async (context, next) => {
            const membersAdded = context.activity.membersAdded ?? [];
            for (let cnt = 0; cnt < membersAdded.length; cnt++) {
                // Greet anyone that was not the target (recipient) of this message.
                if (membersAdded[cnt].id !== context.activity.recipient.id) {
                    await context.sendActivity(MessageFactory.text('Hello and welcome!'));
                    await runDialog(this.dialog, context, conversationState.createProperty('DialogState'));
                }
            }

            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });
    }

    /**
     * Override the ActivityHandler.run() method to save state changes after the bot logic completes.
     */
    async run(context) {
        await super.run(context);

        // Save any state changes. The load happened during the execution of the Dialog.
        await this.conversationState.saveChanges(context, false);
    }
}

module.exports.RootBot = RootBot;
