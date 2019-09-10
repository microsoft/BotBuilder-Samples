// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityHandler } = require('botbuilder');
const CustomRecognizer = require('./customRecognizer');

class MyBot extends ActivityHandler {
    constructor() {
        super();
        // See https://aka.ms/about-bot-activity-message to learn more about the message and other activity types.
        this.onMessage(async (context, next) => {

            // pass the response through custom recognizer to detect email addresses
            const recognizer = new CustomRecognizer();
            const recognizerResult = recognizer.recognize(context.activity.text);
            const intent = recognizerResult.intents.length > 0 ? recognizer.getTopIntent(recognizerResult) : '';
            
            // respond according to the top intent provided by the recognizer
            if (intent) {
              switch(intent) {
                case 'SendEmail':
                  await context.sendActivity('Your response includes an email address. Would you like to contact us by email?');
                  break;
                case 'SearchForEmailAddress':
                  await context.sendActivity('Your response includes an email address. Would you like to search for an email address?');
                  break;
                case 'OpenEmailAccount':
                  await context.sendActivity('Your response includes an email address. Would you like to open an email account?');
                  break;
                default:
                  await context.sendActivity('Your response includes an email address.'); 
              }
            }

            await context.sendActivity(`You said '${ context.activity.text }'`);
            
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
}

module.exports.MyBot = MyBot;
