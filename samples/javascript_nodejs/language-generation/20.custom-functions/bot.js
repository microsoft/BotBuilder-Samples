// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityHandler, ActivityFactory, MessageFactory } = require('botbuilder');
const { Templates } = require('botbuilder-lg');
const { Expression, ReturnType, ExpressionEvaluator, ExpressionFunctions } = require('adaptive-expressions');
const path = require('path');


// prefix all functions with your namespace to avoid collisions.
const mySqrtFnName = "contoso.sqrt";

class EchoBot extends ActivityHandler {
    constructor() {
        super();
        
        let lgFilePath = path.join(__dirname, './resources/main.lg')

        // add the custom function
        Expression.functions.add(mySqrtFnName, (args) => {
            let retValue = null;
            if (args[0] !== null)
            {
                try {
                    let dblValue = parseFloat(args[0]);
                    retValue = Math.sqrt(dblValue)
                }
                catch(ex) {
                    return retValue;
                }
            }
            return retValue;
        });

        // by default this uses Expression.functions.
        this.lgTemplates = Templates.parseFile(lgFilePath);

        // See https://aka.ms/about-bot-activity-message to learn more about the message and other activity types.
        this.onMessage(async (context, next) => {
            var replyText = this.lgTemplates.evaluate("sqrtReadBack", context.activity);

            await context.sendActivity(ActivityFactory.fromObject(replyText))            
            
            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });

        this.onMembersAdded(async (context, next) => {
            const membersAdded = context.activity.membersAdded;
            const welcomeText = "Hello and welcome! I will return a square root if you typed a number. Will echo everything back.";
            for (let cnt = 0; cnt < membersAdded.length; ++cnt) {
                if (membersAdded[cnt].id !== context.activity.recipient.id) {
                    await context.sendActivity(MessageFactory.text(welcomeText, welcomeText));
                }
            }
            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });
    }
}

module.exports.EchoBot = EchoBot;
