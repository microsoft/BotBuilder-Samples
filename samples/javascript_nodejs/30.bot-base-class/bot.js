// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { BotBase } = require('./botBase');

class MyBot extends BotBase {
    async onMessage(turnContext) {
        await turnContext.sendActivity(`You said '${ turnContext.activity.text }'`);
    }
    async onMemberAdded(member, turnContext) {
        await turnContext.sendActivity(`Welcome ${member.name}`);
    }
}

module.exports.MyBot = MyBot;
