// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { TextPrompt } = require('botbuilder-dialogs');
const { MessageFactory } = require('botbuilder');
// LUIS service type entry for the get user profile LUIS model.
const LUIS_CONFIGURATION = 'getUserProfile';

// LUIS intent names from ./resources/getUserProfile.lu
const WHY_DO_YOU_ASK_INTENT = 'Why_do_you_ask';
const GET_USER_NAME_INTENT = 'Get_user_name';
const NO_NAME_INTENT = 'No_Name';
const NONE_INTENT = 'None';
// User name entity from ./resources/getUserProfile.lu
const USER_NAME = 'userName_patternAny';
const { LuisRecognizer } = require('botbuilder-ai');
const userProfile = require('../shared/stateProperties/userProfileProperty');
// This is a custom TextPrompt that requires the input to be between 1 and 50 characters in length.
module.exports = class GetUserNamePrompt extends TextPrompt {
    constructor(dialogId, botConfig, userProfilePropertyAccessor, turnCounterPropertyAccessor, onTurnPropertyAccessor) {
        if(!botConfig) throw ('Need bot configuration');
        if(!userProfilePropertyAccessor) throw ('Need user profile property accessor');
        super(dialogId, async (turnContext, step) => { 
            let turnCounter = await this.turnCounterPropertyAccessor.get(turnContext);
            turnCounter = (turnCounter === undefined) ? 0 : ++turnCounter;
            
            // We are not going to spend more than 5 turns to get user's name.
            if(turnCounter >= 5) {
                await turnContext.sendActivity(`Sorry. This is not working. Hello Human, nice to meet you!`);
                await turnContext.sendActivity(`You can always say 'My name is <your name>' to introduce yourself to me.`);
                step.end();
            }
            this.turnCounterPropertyAccessor.set(turnContext, turnCounter);
            // TODO: increment and set turn counter at right place.
            if(!step.recognized) {
                await turnContext.sendActivity(`Please tell me your name`);
            } else {
                const value = step.recognized.value;
                // call LUIS and get results
                const LUISResults = await this.luisRecognizer.recognize(turnContext); 
                const topIntent = LuisRecognizer.topIntent(LUISResults);
                // did user ask for help or said they are not going to give us the name? 
                switch(topIntent) {
                    case NO_NAME_INTENT: {
                        // set user name in profile to Human
                        this.userProfilePropertyAccessor.set(turnContext, new userProfile('Human'));
                        await turnContext.sendActivity(`No worries. Hello Human, nice to meet you!`);
                        await turnContext.sendActivity(`You can always say 'My name is <your name>' to introduce yourself to me.`);
                        step.end(value);
                        break;
                    }
                    case GET_USER_NAME_INTENT: {
                        if(USER_NAME in LUISResults.entities) {
                            const userName = LUISResults.entities[USER_NAME][0];
                            this.userProfilePropertyAccessor.set(turnContext, new userProfile(userName));
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
                        const userName = value;
                        this.userProfilePropertyAccessor.set(turnContext, new userProfile(userName));
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
        this.botConfig = botConfig;
        this.userProfilePropertyAccessor = userProfilePropertyAccessor;
        this.turnCounterPropertyAccessor = turnCounterPropertyAccessor;
        // add recogizers
        const luisConfig = botConfig.findServiceByNameOrId(LUIS_CONFIGURATION);
        if(!luisConfig || !luisConfig.appId) throw (`Get User Profile LUIS configuration not found in .bot file. Please ensure you have all required LUIS models created and available in the .bot file. See readme.md for additional information\n`);
        this.luisRecognizer = new LuisRecognizer({
            applicationId: luisConfig.appId,
            azureRegion: luisConfig.region,
            // CAUTION: Its better to assign and use a subscription key instead of authoring key here.
            endpointKey: luisConfig.authoringKey
        });
    }
    // async onPrompt(context, state, options, isRetry) {
    //     // Over ride onPrompt to pass in luis recognizer and user profile property accessor.
    //     //state.luisRecognizer = this.luisRecognizer;
    //     //state.userProfilePropertyAccessor = this.userProfilePropertyAccessor;
    //     //super(context, state, options, isRetry);
    //     state.base = this;
    // }
    // async validateResults(turnContext, step) {
    //     if (!step.recognized) {
    //         await turnContext.sendActivity('Please tell me your name.');
    //     } else {
    //         const value = step.recognized.value;
    //         if (value.length < 1) {
    //             await turnContext.sendActivity('Your name has to include at least one character.');
    //         } else if (value.length > 50) {
    //             await turnContext.sendActivity(`Sorry, but I can only handle names of up to 50 characters. Yours was ${ value.length }.`);
    //         } else {
    //             step.end(value);
    //         }
    //     }
    // }
}