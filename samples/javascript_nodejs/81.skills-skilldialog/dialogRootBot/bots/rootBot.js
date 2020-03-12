// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityHandler, CardFactory } = require('botbuilder');
const WelcomeCard = require('../cards/welcomeCard.json');

class RootBot extends ActivityHandler {
    /**
     *
     * @param {ConversationState} conversationState
     * @param {Dialog} dialog
     */
    constructor(conversationState, dialog) {
        super();
        if (!conversationState) throw new Error('[RootBot]: Missing parameter. conversationState is required');
        if (!dialog) throw new Error('[RootBot]: Missing parameter. dialog is required');

        this.conversationState = conversationState;
        this.dialog = dialog;

        this.onMessage(async (context, next) => {
            await dialog.run(context, this.conversationState.createProperty('DialogState'));

            await next();
        });

        this.onMembersAdded(async (context, next) => {
            const membersAdded = context.activity.membersAdded;
            for (let cnt = 0; cnt < membersAdded.length; cnt++) {
                if (membersAdded[cnt].id !== context.activity.recipient.id) {
                    const welcomeCard = CardFactory.adaptiveCard(WelcomeCard);
                    await context.sendActivity({ attachments: [welcomeCard] }, 'Welcome to the Dialog Skill Sample!');
                    await dialog.run(context, conversationState.createProperty('DialogState'));
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
