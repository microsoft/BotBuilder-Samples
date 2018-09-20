// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { CardFactory } = require('botbuilder');

const mainState = require('./mainState');
const Greeting = require('../greeting');
const WelcomeCard = require('./resources/botFrameworkWelcomeCard.json');


class MainDialog {
    constructor(conversationState, botConfig) {
        this.state = new mainState(conversationState);
        this.greeting = new Greeting(conversationState, botConfig);
    }

    async onTurn(context) {
        if (context.activity.type === 'conversationUpdate' && context.activity.membersAdded[0].name === 'Bot') {
            // When activity type is "conversationUpdate" and the member joining the conversation is the bot
            // we will send our Welcome Adaptive Card.  This will only be sent once, when the Bot joins conversation
            // To learn more about Adaptive Cards, see https://aka.ms/msbot-adaptivecards for more details.
            const welcomeCard = CardFactory.adaptiveCard(WelcomeCard);
            await context.sendActivity({ attachments: [welcomeCard] });
        } else if (context.activity.type === 'message') {

            // call LUIS recognizer
            await greeting.onTurn(context);

            // read from state.
            let count = await this.state.get(context);
            count = count === undefined ? 0 : count;
            await context.sendActivity(`${count}: You said "${context.activity.text}"`);
            // increment and set turn counter.
            this.state.set(context, ++count);
        }
    }
}

module.exports = MainDialog;