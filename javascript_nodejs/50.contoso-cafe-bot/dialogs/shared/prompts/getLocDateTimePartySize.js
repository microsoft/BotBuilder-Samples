// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
const { TextPrompt } = require('botbuilder-dialogs');
const { MessageFactory } = require('botbuilder');
const { LuisRecognizer } = require('botbuilder-ai');

// Dialog name from ../../bookTable/resources/turn-N.lu
const PROMPT_NAME = 'GetLocationDateTimePartySize';

// LUIS service type entry for turn.n book table LUIS model in the .bot file.
const LUIS_CONFIGURATION = 'cafeBotBookTableTurnN';

const { ReservationOutcome, ReservationResult, reservationStatus } = require('../createReservationPropertyResult');

const { reservationProperty } = require('../stateProperties');

const { onTurnProperty } = require('../stateProperties');

// This is a custom TextPrompt that uses a LUIS model to handle turn.n conversations including interruptions.
module.exports = class GetLocDateTimePartySizePrompt extends TextPrompt {

    constructor(dialogId, botConfig, reservationsPropertyAccessor, onTurnPropertyAccessor) {
        if(!dialogId) throw ('Need dialog ID');
        if(!botConfig) throw ('Need bot configuration');
        if(!reservationsPropertyAccessor) throw ('Need user reservation property accessor');
        if(!onTurnPropertyAccessor) throw ('Need on turn property accessor');
        super(dialogId, async (turnContext, step) => { 
            return await this.processInput(turnContext);
        });
        this.reservationsPropertyAccessor = reservationsPropertyAccessor;
        this.onTurnPropertyAccessor = onTurnPropertyAccessor;
        
        // add recogizers
        const luisConfig = botConfig.findServiceByNameOrId(LUIS_CONFIGURATION);
        if(!luisConfig || !luisConfig.appId) throw (`Book Table Turn.N LUIS configuration not found in .bot file. Please ensure you have all required LUIS models created and available in the .bot file. See readme.md for additional information\n`);
        this.luisRecognizer = new LuisRecognizer({
            applicationId: luisConfig.appId,
            azureRegion: luisConfig.region,
            // CAUTION: Its better to assign and use a subscription key instead of authoring key here.
            endpointKey: luisConfig.authoringKey
        });
    }

    //onPrompt(context: TurnContext, state: any, options: PromptOptions, isRetry: boolean)
    async onPrompt(context, state, options, isRetry) {
        // override
        await context.sendActivity('Book table!');
        return await this.processInput(context);
    }

    async processInput(turnContext) {
        // get reservation property
        let newReservation = await this.reservationsPropertyAccessor.get(turnContext);

        if(newReservation === undefined) newReservation = new reservationProperty();
        
        // get on turn property
        const onTurnProperties = await this.onTurnPropertyAccessor.get(turnContext);

        // if on turn property has entities
        let updateResult;
        if(onTurnProperties !== undefined && onTurnProperties.entities && onTurnProperties.entities.length !== 0) {
            // update reservation property with on turn property results
            if(newReservation !== undefined) {
                updateResult = newReservation.updateProperties(onTurnProperties);
            } else {
                // Static method that returns a reservation property with onTurnproperties passed in.
                updateResult = reservationProperty.fromOnTurnProperty(onTurnProperties);
            }
        }
        // see if updadte reservtion resulted in errors, if so, report them to user. 
        if(updateResult &&
            updateResult.status === reservationStatus.INCOMPLETE &&
            updateResult.outcome !== undefined &&
            updateResult.outcome.length !== 0) {
                // set reservation property 
                this.reservationsPropertyAccessor.set(turnContext, updateResult.newReservation);
                // return and do not continue if there is an error.
                return await turnContext.sendActivity(updateResult.outcome[0].message);
        }
        if(turnContext.activity.text !== undefined) {
            // call LUIS and get results
            const LUISResults = await this.luisRecognizer.recognize(turnContext); 
            // update reservation property with LUIS results
            updateResult = newReservation.updateProperties(onTurnProperty.fromLUISResults(LUISResults));
            
            // TODO: end if LUISResult is an interruption

            
        }
        // see if updadte reservtion resulted in errors, if so, report them to user. 
        if(updateResult &&
            updateResult.status === reservationStatus.INCOMPLETE &&
            updateResult.outcome !== undefined &&
            updateResult.outcome.length !== 0) {
                // set reservation property 
                this.reservationsPropertyAccessor.set(turnContext, updateResult.newReservation);
                // return and do not continue if there is an error.
                return await turnContext.sendActivity(updateResult.outcome[0].message);
        }
        
        // set reservation property
        if(updateResult !== undefined && updateResult.newReservation !== undefined) {
            this.reservationsPropertyAccessor.set(turnContext, updateResult.newReservation);
        }

        // if we have a valid reservation, end this prompt. 
        // else Get LG based on what's available in reservatio property 
        if(newReservation.haveCompleteReservationProperty()) {
            // TODO: this might have to be updated to be turnResult.
            // step.end(newReservation);
            return await turnContext.sendActivity(`have complete reservation`);
        } else {
            // Ask user for missing information
            return await turnContext.sendActivity(newReservation.getMissingPropertyReadOut());
        }
    }
};