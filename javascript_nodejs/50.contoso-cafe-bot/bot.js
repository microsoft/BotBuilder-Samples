// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityTypes, CardFactory, MessageFactory } = require('botbuilder');
const { DialogSet } = require('botbuilder-dialogs');
const { LuisRecognizer } = require('botbuilder-ai');
const { OnTurnProperty } = require('./dialogs/shared/stateProperties');
const { MainDispatcher } = require('./dialogs/mainDispatcher');
const { WelcomeCard } = require('./dialogs/welcome');

// LUIS service type entry in the .bot file for dispatch.
const LUIS_CONFIGURATION = 'cafeDispatchModel';

// State properties
const ON_TURN_PROPERTY = 'onTurnStateProperty';
const DIALOG_STATE_PROPERTY = 'dialogStatePropety';

module.exports = {
    Bot: class {
        /**
         * Bot constructor.
         * 
         * @param {Object} conversationState conversation state object
         * @param {Object} userState user state object
         * @param {Object} botConfig bot configuration
         * 
         */
        constructor (conversationState, userState, botConfig) {
            if (!conversationState) throw ('Missing parameter. Conversation state is required.');
            if (!userState) throw ('Missing parameter. User state is required.');
            if (!botConfig) throw ('Missing parameter. Bot configuration is required.');

            // Create state property accessors.
            this.onTurnAccessor = conversationState.createProperty(ON_TURN_PROPERTY);
            this.dialogAccessor = conversationState.createProperty(DIALOG_STATE_PROPERTY);
            
            // add recogizers
            const luisConfig = botConfig.findServiceByNameOrId(LUIS_CONFIGURATION);
            if (!luisConfig || !luisConfig.appId) throw (`Cafe Dispatch LUIS model not found in .bot file. Please ensure you have all required LUIS models created and available in the .bot file. See readme.md for additional information.\n`);
            this.luisRecognizer = new LuisRecognizer({
                applicationId: luisConfig.appId,
                azureRegion: luisConfig.region,
                // CAUTION: Its better to assign and use a subscription key instead of authoring key here.
                endpointKey: luisConfig.subscriptionKey
            });

            // add main dialog
            this.dialogs = new DialogSet(this.dialogAccessor);
            this.dialogs.add(new MainDispatcher(botConfig, this.onTurnAccessor, conversationState, userState));
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
                    let onTurnProperties = await this.detectIntentAndEntities(context);
                    if(onTurnProperties === undefined) break;
                    
                    // Set the state with gathered properties (intent/ entities) through the onTurnAccessor
                    await this.onTurnAccessor.set(context, onTurnProperties);
                    
                    // Do we have any oustanding dialogs? if so, continue them and get results
                    // No active dialog? start a new main dialog
                    // Create dialog context.
                    const dc = await this.dialogs.createContext(context);
                    
                    // Continue outstanding dialogs. 
                    await dc.continue();
                    
                    // Begin main dialog if no oustanding dialogs/ no one responded
                    if (!dc.context.responded) {
                        await dc.begin(MainDispatcher.Name);
                    }
                    break;
                }
                case ActivityTypes.ConversationUpdate: {
                    if (context.activity.membersAdded.length !== 0) {
                        // Iterate over all new members added to the conversation
                        // Greet anyone that was not the target (recipient) of this message
                        // the 'bot' is the recipient for events from the channel,
                        // turnContext.Activity.MembersAdded == turnContext.Activity.Recipient.Id indicates the
                        // bot was added to the conversation.
                        // TODO: Send welcome card the right way
                        if(context.activity.membersAdded[0].name !== 'Bot') await this.welcomeUser(context);
                    }
                    break;
                }
                default: {
                    // Handle other activity types as needed.
                    break;            
                }
            }
        }
        /**
         * Async helper method to get on turn properties from cards or NLU using https://LUIS.ai
         * 
         * @param {Object} context conversation context object
         * 
         */
        async detectIntentAndEntities (context) {
            // Handle card input (if any), update state and return.
            if (context.activity.value !== undefined) {
                return OnTurnProperty.fromCardInput(context.activity.value);
            }
            
            // Acknowledge attachments from user. 
            if (context.activity.attachments && context.activity.attachments.length !== 0) {
                await context.sendActivity(`Thanks for sending me that attachment. I'm still learning to process attachments.`);
                return undefined;
            }

            // Nothing to do for this turn if there is no text specified.
            if (context.activity.text === undefined || context.activity.text.trim() === '') {
                return;
            }

            // Call to LUIS recognizer to get intent + entities
            const LUISResults = await this.luisRecognizer.recognize(context);

            // Return new instance of on turn property from LUIS results.
            return OnTurnProperty.fromLUISResults(LUISResults);
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
            await context.sendActivity(MessageFactory.attachment(CardFactory.adaptiveCard(WelcomeCard)));
        }
    }
};