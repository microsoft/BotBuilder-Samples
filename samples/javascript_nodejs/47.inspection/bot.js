// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityHandler } = require('botbuilder');

class IntersectionBot extends ActivityHandler {
    constructor(conversationState, userState) {
        super();
        this.conversationState = conversationState;
        this.conversationStateAccessor = this.conversationState.createProperty('test-conversation-state');
        this.userState = userState;
        this.userStateAccessor = this.userState.createProperty('test-user-state');

        this.onMessage(async (context, next) => {
            // get the state objects
            const testConversationState = await this.conversationStateAccessor.get(context, { count: 0 });
            const testUserState = await this.userStateAccessor.get(context, { count: 0 });

            // print the current state to the reply to show we are incrementing it
            await context.sendActivity(`You said '${ context.activity.text }' conversation-state: ${ testConversationState.count } user-state: ${ testUserState.count }`);

            // do the actual increment
            testConversationState.count++;
            testUserState.count++;

            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });

        this.onMembersAdded(async (context, next) => {
            const membersAdded = context.activity.membersAdded;
            for (let cnt = 0; cnt < membersAdded.length; ++cnt) {
                if (membersAdded[cnt].id !== context.activity.recipient.id) {
                    await context.sendActivity('Hello and welcome!');
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
        await this.userState.saveChanges(context, false);
    }
}

module.exports.IntersectionBot = IntersectionBot;
