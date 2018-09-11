// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
const { TextPrompt } = require('botbuilder-dialogs');
const { MessageFactory } = require('botbuilder');
const { LuisRecognizer } = require('botbuilder-ai');

// Dialog name from ../../bookTable/resources/turn-N.lu
const PROMPT_NAME = 'GetLocationDateTimePartySize';

// LUIS service type entry for turn.n book table LUIS model in the .bot file.
const LUIS_CONFIGURATION = 'cafeBotBookTableTurnN';
const turnResult = require('../turnResult');

const { ReservationOutcome, ReservationResult, reservationStatus } = require('../createReservationPropertyResult');
const { DialogTurnStatus } = require('botbuilder-dialogs');

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
            // get reservation property
            let reservationFromState = await this.reservationsPropertyAccessor.get(turnContext);
            let newReservation; 

            if(reservationFromState === undefined) {
                newReservation = new reservationProperty(); 
            } else {
                newReservation = reservationProperty.fromJSON(reservationFromState);
            }
            
            // get on turn property
            const onTurnProperties = await this.onTurnPropertyAccessor.get(turnContext);

            // if on turn property has entities
            let updateResult;
            if(onTurnProperties !== undefined && onTurnProperties.entities && onTurnProperties.entities.length !== 0) {
                // update reservation property with on turn property results
                updateResult = newReservation.updateProperties(onTurnProperties);
            }
            // see if updadte reservtion resulted in errors, if so, report them to user. 
            if(updateResult &&
                updateResult.status === reservationStatus.INCOMPLETE &&
                updateResult.outcome !== undefined &&
                updateResult.outcome.length !== 0) {
                    // set reservation property 
                    this.reservationsPropertyAccessor.set(turnContext, updateResult.newReservation);
                    // return and do not continue if there is an error.
                    await turnContext.sendActivity(updateResult.outcome[0].message);
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
                    await turnContext.sendActivity(updateResult.outcome[0].message);
            }
            
            // set reservation property
            if(updateResult !== undefined && updateResult.newReservation !== undefined) {
                this.reservationsPropertyAccessor.set(turnContext, updateResult.newReservation);
            }

            // if we have a valid reservation, end this prompt. 
            // else Get LG based on what's available in reservatio property 
            if(newReservation.haveCompleteReservationProperty()) {
                //return new turnResult(DialogTurnStatus.complete);
                step.end(DialogTurnStatus.complete);
            } else {
                // Ask user for missing information
                await turnContext.sendActivity(newReservation.getMissingPropertyReadOut());
            }
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
};