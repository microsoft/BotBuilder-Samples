// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityHandler } = require('botbuilder');
const { LuisRecognizer, QnAMaker } = require('botbuilder-ai');
const { OrchestratorRecognizer } = require('botbuilder-ai-orchestrator');

class DispatchBot extends ActivityHandler {
    constructor() {
        super();

        const dispatchRecognizer = new OrchestratorRecognizer().configure({
            modelPath: process.env.ModelPath, 
            snapshotPath: process.env.SnapShotPath
        });

        const weatherLuisRecognizer = new LuisRecognizer({
            applicationId: process.env.WeatherLuisAppId,
            endpointKey: process.env.LuisAPIKey,
            endpoint: process.env.LuisAPIHostName
        }, {
            includeAllIntents: true,
            includeInstanceData: true
        }, true);

        const homeAutomationLuisRecognizer = new LuisRecognizer({
            applicationId: process.env.HomeAutomationLuisAppId,
            endpointKey: process.env.LuisAPIKey,
            endpoint: process.env.LuisAPIHostName
        }, {
            includeAllIntents: true,
            includeInstanceData: true
        }, true);

        const qnaMaker = new QnAMaker({
            knowledgeBaseId: process.env.QnAKnowledgebaseId,
            endpointKey: process.env.QnAEndpointKey,
            host: process.env.QnAEndpointHostName
        });

        this.dispatchRecognizer = dispatchRecognizer;
        this.qnaMaker = qnaMaker;
        this.weatherLuisRecognizer = weatherLuisRecognizer;
        this.homeAutomationLuisRecognizer = homeAutomationLuisRecognizer;

        this.onMessage(async (context, next) => {
            console.log('Processing Message Activity.');

            // First, we use the dispatch model to determine which cognitive service (LUIS or QnA) to use.
            const recognizerResult = await dispatchRecognizer.recognize(context);

            // Top intent tell us which cognitive service to use.
            const intent = LuisRecognizer.topIntent(recognizerResult);

            // Next, we call the dispatcher with the top intent.
            await this.dispatchToTopIntentAsync(context, intent, recognizerResult);

            await next();
        });

        this.onMembersAdded(async (context, next) => {
            const welcomeText = 'Type a greeting or a question about the weather to get started.';
            const membersAdded = context.activity.membersAdded;

            for (const member of membersAdded) {
                if (member.id !== context.activity.recipient.id) {
                    await context.sendActivity(`Welcome to Dispatch bot ${ member.name }. ${ welcomeText }`);
                }
            }

            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });
    }

    async dispatchToTopIntentAsync(context, intent, recognizerResult) {
        switch (intent) {
        case 'HomeAutomation':
            await this.processHomeAutomation(context);
            break;
        case 'Weather':
            await this.processWeather(context);
            break;
        case 'QnAMaker':
            await this.processSampleQnA(context);
            break;
        default:
            console.log(`Dispatch unrecognized intent: ${ intent }.`);
            await context.sendActivity(`Dispatch unrecognized intent: ${ intent }.`);
            break;
        }
    }

    async processHomeAutomation(context) {
        console.log('processHomeAutomation');

        // Retrieve LUIS result for Process Automation.
        const luisResult = await this.homeAutomationLuisRecognizer.recognize(context);
        // Top intent tell us which cognitive service to use.
        const topIntent = LuisRecognizer.topIntent(luisResult);

        await context.sendActivity(`HomeAutomation top intent ${ topIntent }.`);
    }

    async processWeather(context) {
        console.log('processWeather');

        // Retrieve LUIS results for Weather.
        const luisResult = await this.weatherLuisRecognizer.recognize(context);
        // Top intent tell us which cognitive service to use.
        const topIntent = LuisRecognizer.topIntent(luisResult);

        await context.sendActivity(`ProcessWeather top intent ${ topIntent }.`);
    }

    async processSampleQnA(context) {
        console.log('processSampleQnA');

        const results = await this.qnaMaker.getAnswers(context);

        if (results.length > 0) {
            await context.sendActivity(`${ results[0].answer }`);
        } else {
            await context.sendActivity('Sorry, could not find an answer in the Q and A system.');
        }
    }
}

module.exports.DispatchBot = DispatchBot;
