// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityTypes } = require('botbuilder');

class BotBase {
    async onTurn(turnContext) {
        switch (turnContext.activity.type) {
            case ActivityTypes.Message:
                await this.onMessage(turnContext);
                break;
            case ActivityTypes.ConversationUpdate:
                await this.onConversationUpdate(turnContext);
                break;
            // TODO: add other activity types here
        }
    }

    async onMessage(turnContext) {
    }

    async onConversationUpdate(turnContext) {
        if (turnContext.activity.hasOwnProperty('membersAdded')) {

            for (let i=0; i<turnContext.activity.membersAdded.length; i+=1) {
                const member = turnContext.activity.membersAdded[i];
                if (member.id != turnContext.activity.recipient.id)
                {
                    await this.onMemberAdded(member, turnContext);
                }
            }
        }
        // TODO: similar for membersRemoved
    }

    async onMemberAdded(member, turnContext) {
    }
}

module.exports.BotBase = BotBase;
