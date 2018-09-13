// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityTypes, CardFactory } = require('botbuilder');
const { DialogTurnStatus, DialogSet } = require('botbuilder-dialogs');
const { LuisRecognizer } = require('botbuilder-ai');

const { onTurnProperty } = require('./dialogs/shared/stateProperties');
const MainDialog = require('./dialogs/mainDialog');
const welcomeCard = require('./dialogs/welcome');

// LUIS service type entry in the .bot file for dispatch.
const LUIS_CONFIGURATION = 'cafeDispatchModel';

// State properties
const ON_TURN_PROPERTY = 'onTurnStateProperty';
const DIALOG_STATE_PROPERTY = 'dialogStatePropety';

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
        if(!conversationState) throw ('Missing parameter. conversationState is required');
        if(!userState) throw ('Missing parameter. userState is required');
        if(!botConfig) throw ('Missing parameter. botConfig is required');

        // Create state property accessors.
        this.onTurnPropertyAccessor = conversationState.createProperty(ON_TURN_PROPERTY);
        this.dialogPropertyAccessor = conversationState.createProperty(DIALOG_STATE_PROPERTY);
        
        // add recogizers
        const luisConfig = botConfig.findServiceByNameOrId(LUIS_CONFIGURATION);
        if(!luisConfig || !luisConfig.appId) throw (`Cafe Dispatch LUIS model not found in .bot file. Please ensure you have all required LUIS models created and available in the .bot file. See readme.md for additional information\n`);
        this.luisRecognizer = new LuisRecognizer({
            applicationId: luisConfig.appId,
            azureRegion: luisConfig.region,
            // CAUTION: Its better to assign and use a subscription key instead of authoring key here.
            endpointKey: luisConfig.subscriptionKey
        });

        // add main dialog
        this.dialogs = new DialogSet(this.dialogPropertyAccessor);
        this.dialogs.add(new MainDialog(botConfig, this.onTurnPropertyAccessor, conversationState, userState));
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
                // OnTurnProperty object has processed information from the input message activity.
                let onTurnProperties = await this.getNewOnTurnProperties(context);
                if(onTurnProperties === undefined) break;
                
                // Set the state with gathered properties (intent/ entities) through the onTurnPropertyAccessor
                this.onTurnPropertyAccessor.set(context, onTurnProperties);
                
                // Do we have any oustanding dialogs? if so, continue them and get results
                // No active dialog? start a new main dialog
                await this.continueOrBeginMainDialog(context);
                break;
            }
            case ActivityTypes.ConversationUpdate: {
                
                // Send a welcome card to any user that joins the conversation.
                if(context.activity.membersAdded[0].name !== 'Bot') await this.welcomeUser(context);
                break;
            }
            default: {
                
                // Handle other acivity types as needed.
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
        // Create dialog context.
        let dc = await this.dialogs.createContext(context);
        
        // Continue outstanding dialogs. 
        let result = await dc.continue();
        
        // If no oustanding dialogs, begin main dialog
        if(result.status === DialogTurnStatus.empty) {
            await dc.begin(MainDialog.Name);
        }
    }
    /**
     * Async helper method to get on turn properties from cards or NLU using https://LUIS.ai
     * 
     * @param {Object} context conversation context object
     * 
     */
    async getNewOnTurnProperties (context) {
        // Handle card input (if any), update state and return.
        if(context.activity.value !== undefined) return onTurnProperty.fromCardInput(context.activity.value);
        
        // Acknowledge attachments from user. 
        if(context.activity.attachments && context.activity.attachments.length !== 0) {
            await context.sendActivity(`Thanks for sending me that attachment. I'm still learning to process attachments.`);
            return undefined;
        }

        // Nothing to do for this turn if there is no text specified.
        if(context.activity.text === undefined || context.activity.text.trim() === '') return;

        // make call to LUIS recognizer to get intent + entities
        const LUISResults = await this.luisRecognizer.recognize(context);

        // Return new instance of on turn property from LUIS results.
        return onTurnProperty.fromLUISResults(LUISResults);
    }
    /**
     * Async helper method to welcome the user.
     * 
     * @param {Object} context conversation context object
     * 
     */
    async welcomeUser (context) {
        // Welcome user.
        await context.sendActivity(`Hello, I am the Contoso Cafe Bot!`);
        await context.sendActivity(`I can help book a table, find cafe locations and more..`);
        
        // Send welcome card.
        await context.sendActivity({ attachments: [CardFactory.adaptiveCard(welcomeCard)]});
    }
}

module.exports = Bot;