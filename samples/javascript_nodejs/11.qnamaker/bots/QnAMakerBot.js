// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityHandler } = require('botbuilder');
const { QnAMaker } = require('botbuilder-ai');
const { ActivityTypes } = require('botframework-schema');

class QnAMakerBot extends ActivityHandler {
    /**
     * The QnAMakerBot constructor requires one argument (`endpoint`) which is used to create an instance of `QnAMaker`.
     * @param {QnAMakerEndpoint} qnaEndpointSettings The basic configuration needed to call QnA Maker. In this sample the configuration is retrieved from the .env file.
     * @param {QnAMakerOptions} config An optional parameter that contains additional settings for configuring a `QnAMaker` when calling the service.
     * @param {any} logger object for logging events, defaults to console if none is provided
     */
    constructor(qnaEndpointSettings, qnaOptions, logger) {
        super();
        if (!qnaEndpointSettings) throw new Error('[QnaMakerBot]: Missing parameter. qnaEndpointSettings is required');
        if (!logger) {
            logger = console;
            logger.log('[QnaMakerBot]: logger not passed in, defaulting to console');
        }

        this.qnaMaker = new QnAMaker(qnaEndpointSettings, qnaOptions);
        this.logger = logger;

        // If a new user is added to the conversation, send them a greeting message
        this.onMembersAdded(async (context, next) => {
            const membersAdded = context.activity.membersAdded;
            for (let cnt = 0; cnt < membersAdded.length; cnt++) {
                if (membersAdded[cnt].id !== context.activity.recipient.id) {
                    await context.sendActivity('Welcome to the QnA Maker sample! Ask me a question and I will try to answer it.');
                }
            }

            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });

        // When a user sends a message, perform a call to the QnA Maker service to retrieve matching Question and Answer pairs.
        this.onMessage(async (context, next) => {
            // Show "..." from the bot so it looks like the bot is typing while it searches for an answer
            await context.sendActivity({
                type: ActivityTypes.Typing
            });

            this.logger.log('Calling QnA Maker');

            const qnaResults = await this.qnaMaker.getAnswers(context);

            // If an answer was received from QnA Maker, send the answer back to the user.
            if (qnaResults[0]) {
                await context.sendActivity(qnaResults[0].answer);

            // If no answers were returned from QnA Maker, reply with help.
            } else {
                await context.sendActivity('No QnA Maker answers were found. This example uses a QnA Maker Knowledge Base that focuses on smart light bulbs. To see QnA Maker in action, ask the bot questions like "Why won\'t it turn on?" or "I need help."');
            }

            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });
    }
}

module.exports.QnAMakerBot = QnAMakerBot;
