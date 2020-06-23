// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { TeamsActivityHandler, MessageFactory, CardFactory } = require('botbuilder');

class TeamsTaskModuleBot extends TeamsActivityHandler {
    constructor() {
        super();
        // See https://aka.ms/about-bot-activity-message to learn more about the message and other activity types.
        this.onMessage(async (context, next) => {
            const card = this.getHeroCardMenu();
            const message = MessageFactory.attachment(card);
            await context.sendActivity(message);

            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });

        this.onMembersAdded(async (context, next) => {
            const card = this.getGetHeroCardMenu();
            const message = MessageFactory.attachment(card);
            await context.sendActivity(message);

            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });
    };

    getHeroCardMenu() {
        return CardFactory.heroCard('Task Module Invocation from Hero Card',
            'This is a hero card with a Task Module Action button.  Click the button to show an Adaptive Card within a Task Module.',
            null, // No images
            [{ type: 'invoke', title: 'Task Module', value: { type: 'task/fetch', data: 'adaptivecard' } }]);
    }

    handleTeamsTaskModuleFetch(context, taskModuleRequest) {
        // taskModuleRequest.data can be checked to determine different paths.

        return {
            task: {
                type: 'continue',
                value: {
                    card: this.getTaskModuleAdaptiveCard(),
                    height: 220,
                    width: 400,
                    title: 'Adaptive Card: Inputs'
                }
            }
        };
    }

    async handleTeamsTaskModuleSubmit(context, taskModuleRequest) {
        return {
            task: {
                type: 'message', // This could also be of type 'continue' with a new Task Module and card.
                value: 'Hello. You said: ' + taskModuleRequest.data.usertext
            }
        };
    }

    getTaskModuleAdaptiveCard() {
        return CardFactory.adaptiveCard({
            version: '1.0.0',
            type: 'AdaptiveCard',
            body: [
                {
                    type: 'TextBlock',
                    text: 'Enter Text Here'
                },
                {
                    type: 'Input.Text',
                    id: 'usertext',
                    placeholder: 'add some text and submit',
                    IsMultiline: true
                }
            ],
            actions: [
                {
                    type: 'Action.Submit',
                    title: 'Submit'
                }
            ]
        });
    }
}

module.exports.TeamsTaskModuleBot = TeamsTaskModuleBot;
