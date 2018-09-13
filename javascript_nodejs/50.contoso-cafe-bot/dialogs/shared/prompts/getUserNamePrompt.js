// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
const { TextPrompt } = require('botbuilder-dialogs');
const { MessageFactory } = require('botbuilder');
const { LuisRecognizer } = require('botbuilder-ai');
const { userProfileProperty } = require('../stateProperties');

// LUIS service type entry for the get user profile LUIS model in the .bot file.
const LUIS_CONFIGURATION = 'getUserProfile';

// LUIS intent names from ./resources/getUserProfile.lu
const WHY_DO_YOU_ASK_INTENT = 'Why_do_you_ask';
const GET_USER_NAME_INTENT = 'Get_user_name';
const NO_NAME_INTENT = 'No_Name';
const NONE_INTENT = 'None';

// User name entity from ./resources/getUserProfile.lu
const USER_NAME = 'userName_patternAny';

// This is a custom TextPrompt that uses a LUIS model to handle turn.n conversations including interruptions.
module.exports = class GetUserNamePrompt extends TextPrompt {
    /**
     * Constructor. 
     * 
     * @param {String} dialogId Dialog ID
     * @param {Object} botConfig Bot configuration
     * @param {Object} userProfilePropertyAccessor accessor for user profile property
     * @param {Object} turnCounterPropertyAccessor accessor for turn counter property
     */
    constructor(dialogId, botConfig, userProfilePropertyAccessor, turnCounterPropertyAccessor) {
        if(!dialogId) throw ('Need dialog ID');
        if(!botConfig) throw ('Need bot configuration');
        if(!userProfilePropertyAccessor) throw ('Need user profile property accessor');
        if(!turnCounterPropertyAccessor) throw ('Need turn counter property accessor');
        super(dialogId, async (turnContext, step) => { 
            // Get turn counter
            let turnCounter = await this.turnCounterPropertyAccessor.get(turnContext);
            turnCounter = (turnCounter === undefined) ? 0 : ++turnCounter;
            
            // We are not going to spend more than 5 turns to get user's name.
            if(turnCounter >= 5) {
                await turnContext.sendActivity(`Sorry. This is not working. Hello Human, nice to meet you!`);
                await turnContext.sendActivity(`You can always say 'My name is <your name>' to introduce yourself to me.`);
                step.end();
            }
            
            // set updated turn counter
            this.turnCounterPropertyAccessor.set(turnContext, turnCounter);
            if(!step.recognized) {
                await turnContext.sendActivity(`Please tell me your name`);
            } else {
                const value = step.recognized.value;
                
                // call LUIS and get results
                const LUISResults = await this.luisRecognizer.recognize(turnContext); 
                const topIntent = LuisRecognizer.topIntent(LUISResults);
                
                // Did user ask for help or said they are not going to give us the name? 
                switch(topIntent) {
                    case NO_NAME_INTENT: {
                        // set user name in profile to Human
                        this.userProfilePropertyAccessor.set(turnContext, new userProfileProperty('Human'));
                        await turnContext.sendActivity(`No worries. Hello Human, nice to meet you!`);
                        await turnContext.sendActivity(`You can always say 'My name is <your name>' to introduce yourself to me.`);
                        step.end(value);
                        break;
                    }
                    case GET_USER_NAME_INTENT: {
                        // Find the user's name from LUIS entities list.
                        if(USER_NAME in LUISResults.entities) {
                            let userName = LUISResults.entities[USER_NAME][0];
                            
                            // capitalize user name   
                            userName = userName.charAt(0).toUpperCase() + userName.slice(1);
                            this.userProfilePropertyAccessor.set(turnContext, new userProfileProperty(userName));
                            await turnContext.sendActivity(`Hey there ${userName}!. Nice to meet you!`);
                            step.end(value);
                        } else {
                            await turnContext.sendActivity(`Sorry, I didn't get that. What's your name?`);
                        }
                        break;
                    }
                    case WHY_DO_YOU_ASK_INTENT: {
                        await turnContext.sendActivity(`I need your name to be able to address you correctly!`);
                        await turnContext.sendActivity(MessageFactory.suggestedActions([`I won't give you my name`], `What is your name?`));
                        break;
                    }
                    case NONE_INTENT: {
                        let userName = value;
                        
                        // capitalize user name   
                        userName = userName.charAt(0).toUpperCase() + userName.slice(1);
                        this.userProfilePropertyAccessor.set(turnContext, new userProfileProperty(userName));
                        await turnContext.sendActivity(`Hey there ${userName}!. Nice to meet you!`);
                        step.end(value);
                        break;
                    }
                    default: {
                        // Handle interruption. Pass back original payload.
                        let currentPayload = await this.userProfilePropertyAccessor.get(turnContext);
                        step.end({reason: 'Interruption', payload: currentPayload});
                        break;
                    }
                }
            }
        });
        this.userProfilePropertyAccessor = userProfilePropertyAccessor;
        this.turnCounterPropertyAccessor = turnCounterPropertyAccessor;
        
        // add recogizers
        const luisConfig = botConfig.findServiceByNameOrId(LUIS_CONFIGURATION);
        if(!luisConfig || !luisConfig.appId) throw (`Get User Profile LUIS configuration not found in .bot file. Please ensure you have all required LUIS models created and available in the .bot file. See readme.md for additional information\n`);
        this.luisRecognizer = new LuisRecognizer({
            applicationId: luisConfig.appId,
            azureRegion: luisConfig.region,
            // CAUTION: Its better to assign and use a subscription key instead of authoring key here.
            endpointKey: luisConfig.subscriptionKey
        });
    }
}