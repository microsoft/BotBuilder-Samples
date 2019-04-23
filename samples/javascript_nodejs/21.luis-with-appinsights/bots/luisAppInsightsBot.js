// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityHandler } = require('botbuilder');
const { TelemetryLuisRecognizer } = require('../telemetry/telemetryLuisRecognizer');

class LuisAppInsightsBot extends ActivityHandler {
    constructor(logger) {
        super();
        if (!logger) {
            logger = console;
            logger.log('[LuisAppInsightsBot]: logger not passed in, defaulting to console');
        }

        this.luisRecognizer = new TelemetryLuisRecognizer({
            applicationId: process.env.LuisAppId,
            endpointKey: process.env.LuisAPIKey,
            endpoint: `https://${ process.env.LuisAPIHostName }.api.cognitive.microsoft.com`
        }, {
            includeAllIntents: true,
            includeInstanceData: true
        }, true);

        this.logger = logger;

        this.onMessage(async (context, next) => {
            // Perform a call to LUIS to retrieve results for the user's message.
            const recognizerResult = await this.luisRecognizer.recognize(context);

            if (recognizerResult) {
                await this.ProcessLuis(context, recognizerResult);
            } else {
                const msg = `No LUIS intents were found.\n` +
                    `This sample is about identifying two user intents:\n` +
                    `&nbsp;&nbsp;&nbsp;&nbsp;Calendar.Add\n` +
                    `&nbsp;&nbsp;&nbsp;&nbsp;Calendar.Find\n` +
                    `Try typing 'Add Event' or 'Show me tomorrow'.`;
                await context.sendActivity(msg);
            }

            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });

        this.onMembersAdded(async (context, next) => {
            const welcomeText = `This sample is about identifying two user intents:\n` +
            `&nbsp;&nbsp;&nbsp;&nbsp;Calendar.Add\n` +
            `&nbsp;&nbsp;&nbsp;&nbsp;Calendar.Find\n` +
            `Try typing 'Add Event' or 'Show me tomorrow'.`;
            // Send a welcome message
            const membersAdded = context.activity.membersAdded;
            for (let cnt = 0; cnt < membersAdded.length; cnt++) {
                if (membersAdded[cnt].id !== context.activity.recipient.id) {
                    await context.sendActivity(`Welcome to the LUIS with Application Insights Bot, ${ membersAdded[cnt].name }. ${ welcomeText }`);
                }
            }

            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });
    }

    async ProcessLuis(context, recognizerResult) {
        this.logger.log('ProcessLuis');

        const result = recognizerResult;
        const topIntent = result.luisResult.topScoringIntent;

        await context.sendActivity(`Top intent: ${ topIntent.intent }, Score: ${ topIntent.score }`);
        await context.sendActivity(`Intents detected: \n\n ${ Object.keys(result.intents).join('\n\n') }`);
    }
}

module.exports.LuisAppInsightsBot = LuisAppInsightsBot;
