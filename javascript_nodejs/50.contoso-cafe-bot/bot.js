// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityTypes, CardFactory, MessageFactory } = require('botbuilder');
const { DialogTurnStatus, DialogSet } = require('botbuilder-dialogs');
const { LuisRecognizer } = require('botbuilder-ai');

const myBotState = require('./botState');
const MainDialog = require('./dialogs/mainDialog');
const welcomeCard = require('./dialogs/welcome');

// LUIS service type entry in the .bot file for dispatch.
const LUIS_CONFIGURATION = 'cafeDispatchModel';
// Possible LUIS entities. You can refer to dialogs\mainDialog\resources\entities.lu for list of entities
const LUIS_ENTITIES = ['confirmationList', 'number', 'datetimeV2', 'cafeLocation'];

class Bot {
    /**
     * Bot constructor.
     * 
     * @param {Object} conversationState conversation state object
     * @param {Object} userState user state object
     * @param {Object} botConfig bot configuration
     * 
     */
    constructor (conversationState, userState, botConfig) {
        if(!conversationState) throw ('Need converstaion state');
        if(!userState) throw ('Need user state');
        if(!botConfig) throw ('Need bot config');
        // Create state objects for user, conversation and dialog states.   
        this.botState = new myBotState(conversationState);
        this.botConfig = botConfig;
        // add recogizers
        const luisConfig = botConfig.findServiceByNameOrId(LUIS_CONFIGURATION);
        if(!luisConfig || !luisConfig.appId) throw (`Home automation LUIS model not found in .bot file. Please ensure you have all required LUIS models created and available in the .bot file. See readme.md for additional information\n`);
        this.luisRecognizer = new LuisRecognizer({
            applicationId: luisConfig.appId,
            azureRegion: luisConfig.region,
            // CAUTION: Its better to assign and use a subscription key instead of authoring key here.
            endpointKey: luisConfig.authoringKey
        });
        // add dialogs
        this.dialogs = new DialogSet(this.botState.dialogPropertyAccessor);
        this.dialogs.add(new MainDialog(botConfig, this.botState.onTurnPropertyAccessor, conversationState, userState));
    }
    /**
     * On turn dispatcher. Responsible for processing turn input, gather relevant properties,
     * and continues or begins main dialog.
     * 
     * @param {Object} context conversation context object
     * 
     */
    async onTurn (context) {
        // See https://aka.ms/about-bot-activity-message to learn more about message and other activity types.
        switch (context.activity.type) {
            case ActivityTypes.Message: {
                // Process on turn input (card or NLP) and gather new properties
                let onTurnProperties = await this.getNewOnTurnProperties(context);
                // Update state with gathered properties (dialog/ intent/ entities)
                this.botState.onTurnPropertyAccessor.set(context, onTurnProperties);
                // Do we have any oustanding dialogs? if so, continue them and get results
                // No active dialog? start a new main dialog
                await this.continueOrBeginMainDialog(context);
                break;
            }
            case ActivityTypes.ConversationUpdate: {
                if(context.activity.membersAdded[0].name !== 'Bot') await this.welcomeUser(context);
                break;
            }
            default: {
                break;            
            }
        }
    }
    /**
     * Async method to continue or begin main dialog
     * 
     * @param {Object} context conversation context object
     * 
     */
    async continueOrBeginMainDialog(context) {
        // create dialog context;
        let dc = await this.dialogs.createContext(context);
        // Continue outstanding dialogs. 
        let result = await dc.continue();
        // If no oustanding dialogs, begin main dialog
        if(result.status === DialogTurnStatus.empty) {
            await dc.begin(MainDialog.Name);
        }
    }
    /**
     * Async method to get on turn properties from cards or NLU using https://LUIS.ai
     * 
     * @param {Object} context conversation context object
     * 
     */
    async getNewOnTurnProperties (context) {
        // Handle card input (if any), update state and return
        if(context.activity.value !== undefined) return await this.handleCardInput(context.activity.value);
        
        // Nothing to do for this turn if there is no text specified.
        if(context.activity.text === undefined || context.activity.text.trim() === '') return;

        let onTurnProperties = new myBotState.newTurn();
        // make call to LUIS recognizer to get intent + entities
        const LUISResults = await this.luisRecognizer.recognize(context);
        onTurnProperties.intent = LuisRecognizer.topIntent(LUISResults);
        // gather entity values if available
        LUIS_ENTITIES.forEach(luisEntity => {
            if(luisEntity in LUISResults.entities) onTurnProperties.entities.push(myBotState.addNewEntity(luisEntity, LUISResults.entities[luisEntity]))
        });
        return onTurnProperties;
    }
    /**
     * Async method to welcome the user.
     * 
     * @param {Object} context conversation context object
     * 
     */
    async welcomeUser (context) {
        // Welcome user.
        await context.sendActivity(`Hello, I am the Contoso Cafe Bot!`);
        await context.sendActivity(`I can help book a table, find cafe locations and more..`);
        // Welcome card with suggested actions.
        await context.sendActivity({ attachments: [CardFactory.adaptiveCard(welcomeCard)]});
        // TODO: add suggested actions
    }
    /**
     * Async method to process card input and gather turn properties
     * 
     * @param {Object} context conversation context object
     * 
     */
    async handleCardInput (cardValue) {
        let onTurnProperties = new myBotState.newTurn();
        // Add all card values to appropriate place. 
        // All cards used by this bot are adaptive cards with the card's 'data' property set to relevant information.
        for(var key in cardValue) {
            if(!cardValue.hasOwnProperty(key)) continue;
            if(key.toLowerCase().trim() === 'intent') {
                onTurnProperties.intent = cardValue[key];
            } else {
                onTurnProperties.entities.push(myBotState.addNewEntity(key, cardValue[key]));
            }
        }
        return onTurnProperties;
    }
}

module.exports = Bot;