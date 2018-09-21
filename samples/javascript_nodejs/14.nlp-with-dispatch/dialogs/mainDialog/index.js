// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
const { LuisRecognizer } = require('botbuilder-ai');

const homeAutomationDialog = require('../homeAutomation');
const weatherDialog = require('../weather');
const qnaDialog = require('../qna');

// this is the LUIS service type entry in the .bot file.
const DISPATCH_CONFIG = 'nlp-with-dispatchDispatch';

// LUIS intent names. you can get this from the dispatch.lu file.
const HOME_AUTOMATION_INTENT = 'l_homeautomation-LUIS';
const WEATHER_INTENT = 'l_weather-LUIS';
const NONE_INTENT = 'None';
const QNA_INTENT = 'q_sample_qna';

class MainDialog {
    /**
     * 
     * @param {Object} convoState conversation state
     * @param {Object} userState user state
     * @param {Object} botConfig bot configuration from .bot file
     */
    constructor (convoState, userState, botConfig) {
        this.homeAutomationDialog = new homeAutomationDialog(convoState, userState, botConfig);
        this.weatherDialog = new weatherDialog(botConfig);
        this.qnaDialog = new qnaDialog(botConfig);
        
        // dispatch recognizer
        const dispatchConfig = botConfig.findServiceByNameOrId(DISPATCH_CONFIG);
        if(!dispatchConfig || !dispatchConfig.appId) throw (`No dispatch model found in .bot file. Please ensure you have dispatch model created and available in the .bot file. See readme.md for additional information\n`);
        this.dispatchRecognizer = new LuisRecognizer({
            applicationId: dispatchConfig.appId,
            azureRegion: dispatchConfig.region,
            // CAUTION: Its better to assign and use a subscription key instead of authoring key here.
            endpointKey: dispatchConfig.authoringKey
        });
    }
    /**
     * 
     * @param {Object} context context object
     */
    async onTurn(context) {
        if (context.activity.type === 'message') {
            // determine which dialog should fulfill this request
            const dispatchResults = await this.dispatchRecognizer.recognize(context);
            const dispatchTopIntent = LuisRecognizer.topIntent(dispatchResults);
            switch (dispatchTopIntent) {
                case HOME_AUTOMATION_INTENT: 
                    await this.homeAutomationDialog.onTurn(context);
                    break;
                case WEATHER_INTENT:
                    await this.weatherDialog.onTurn(context);
                    break;
                case QNA_INTENT:
                    await this.qnaDialog.onTurn(context);
                    break;
                case NONE_INTENT: 
                default:
                    // Unknown request
                    await context.sendActivity(`I do not understand that.`);
                    await context.sendActivity(`I can help with weather forecast, turning devices on and off and answer general questions like 'hi', 'who are you' etc.`);    
            }            
        }
        else {
            if(context.activity.type === 'conversationUpdate' && context.activity.membersAdded[0].name !== 'Bot') {
                // welcome user
                await context.sendActivity(`Hello, I am the NLP Dispatch bot!`);
                await context.sendActivity(`I can help with weather forecast, turning devices on and off and answer general questions like 'hi', 'who are you' etc.`);
            }
        }
    }
}

module.exports = MainDialog;