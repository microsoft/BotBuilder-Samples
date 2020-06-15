// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { DialogBot } = require('./dialogBot');
const { ActivityFactory } = require('botbuilder');
const { Templates } = require('botbuilder-lg');
class DialogAndWelcomeBot extends DialogBot {
    constructor(conversationState, userState, dialog) {
        super(conversationState, userState, dialog);

        const lgTemplates = Templates.parseFile('./resources/welcomeCard.lg');

        // Actions to include in the welcome card. These are passed to LG and are then included in the generated Welcome card.
        const actions = {
            actions: [
                {
                    title: 'Get an overview',
                    url: 'https://docs.microsoft.com/en-us/azure/bot-service/?view=azure-bot-service-4.0'
                },
                {
                    title: 'Ask a question',
                    url: 'https://stackoverflow.com/questions/tagged/botframework'
                },
                {
                    title: 'Learn how to deploy',
                    url: 'https://docs.microsoft.com/en-us/azure/bot-service/bot-builder-howto-deploy-azure?view=azure-bot-service-4.0'
                }
            ]
        };

        this.onMembersAdded(async (context, next) => {
            const membersAdded = context.activity.membersAdded;
            for (let cnt = 0; cnt < membersAdded.length; cnt++) {
                if (membersAdded[cnt].id !== context.activity.recipient.id) {
                    const welcomeCard = ActivityFactory.fromObject(lgTemplates.evaluate('WelcomeCard', actions));
                    await context.sendActivity(welcomeCard);
                    await dialog.run(context, conversationState.createProperty('DialogState'));
                }
            }

            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });
    }
}

module.exports.DialogAndWelcomeBot = DialogAndWelcomeBot;
