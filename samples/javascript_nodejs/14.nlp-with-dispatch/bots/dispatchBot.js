// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityHandler } = require('botbuilder');
const { LuisRecognizer } = require('botbuilder-ai');

class DispatchBot extends ActivityHandler {
    /**
     * 
     * @param {BotServices} botServices 
     * @param {any} logger object for logging events, defaults to console if none is provided
     */
    constructor(dispatchRecognizer, qnaMaker, logger) {
        super();
        if (!dispatchRecognizer) throw new Error('[DispatchBot]: Missing parameter. dispatchRecognizer is required');
        if (!qnaMaker) throw new Error('[DispatchBot]: Missing parameter. qnaMaker is required');
        if (!logger) {
            logger = console;
            logger.log('[DispatchBot]: logger not passed in, defaulting to console');
        }

        this.logger = logger;
        this.dispatchRecognizer = dispatchRecognizer;
        this.qnaMaker = qnaMaker;

        this.onMessage(async (context, next) => {
            this.logger.log('Processing Message Activity.');

            // First, we use the dispatch model to determine which cognitive service (LUIS or QnA) to use.
            const recognizerResult = await dispatchRecognizer.recognize(context);

            // Top intent tell us which cognitive service to use.
            const topIntent = LuisRecognizer.topIntent(recognizerResult);

            // Next, we call the dispatcher with the top intent.
            await this.dispatchToTopIntentAsync(context, topIntent, recognizerResult);
        });

        this.onMembersAdded(async (context, next) => {
            const welcomeText = 'Type a greeting or a question about the weather to get started.';
            const membersAdded = context.activity.membersAdded;

            for (let member of membersAdded) {
                if (member.id !== context.activity.recipient.id) {
                    await context.sendActivity(`Welcome to Dispatch bot ${member.Name}. ${welcomeText}`);
                }
            }

            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });
    }

    async dispatchToTopIntentAsync(context, topIntent, recognizerResult) {
        switch (topIntent)
        {
            case 'l_HomeAutomation':
                await this.processHomeAutomation(context, recognizerResult.luisResult); 
                break;
            case 'l_Weather':
                await this.processWeather(context, recognizerResult.luisResult);
                break;
            case 'q_sample-qna':
                await this.processSampleQnA(context);
                break;
            default:
                this.logger.log(`Dispatch unrecognized intent: ${topIntent}.`);
                await context.sendActivity(`Dispatch unrecognized intent: ${intent}.`);
                break;
        }
    }

    async processHomeAutomation(context, luisResult) {
        this.logger.log('processHomeAutomation');

        // Retrieve LUIS result for Process Automation.
        const result = luisResult.connectedServiceResult;
        const topIntent = result.topScoringIntent.intent;

        await context.sendActivity(`HomeAutomation top intent ${topIntent}.`);
        // await context.sendActivity(`HomeAutomation intents detected:  ${topIntent}.`); FIGURE OUT WHAT LINQ IS PRINTING
    }

    async processWeather(context, luisResult) {
        this.logger.log('processWeather');

        // Retrieve LUIS results for Weather.
        const result = luisResult.connectedServiceResult;
        const topIntent = result.topScoringIntent.intent;

        await context.sendActivity(`ProcessWeather top intent ${topIntent}.`);
        // await context.sendActivity(`ProcessWeather intents detected:  ${topIntent}.`); FIGURE OUT WHAT LINQ IS PRINTING

        if (luisResult.entities.length > 0) {
            // await context.sendActivity(`ProcessWeather entities were found in the message: PRINT ENTITIES.`); 
        }
    }

    async processSampleQnA(context) {
        this.logger.log('processSampleQnA');

        const results = await this.qnaMaker.getAnswers(context);

        if (results.length > 0) {
            await context.sendActivity(`${results[0].answer}`);
        } else {
            await context.sendActivity('Sorry, could not find an answer in the Q and A system.');
        }
    }
}

module.exports.DispatchBot = DispatchBot;