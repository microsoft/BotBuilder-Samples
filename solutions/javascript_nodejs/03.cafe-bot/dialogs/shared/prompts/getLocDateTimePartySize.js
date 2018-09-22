// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
const { MessageFactory } = require('botbuilder');
const { DialogTurnStatus, TextPrompt } = require('botbuilder-dialogs');
const { LuisRecognizer } = require('botbuilder-ai');
const { Reservation, reservationStatusEnum, OnTurnProperty } = require('../stateProperties');
const { QnADialog } = require('../../qna');
// Dialog name from ../../bookTable/resources/turn-N.lu
const CONTINUE_PROMPT_INTENT = 'GetLocationDateTimePartySize';
const HELP_INTENT = 'Help';
const CANCEL_INTENT = 'Cancel';
const INTERRUPTIONS_INTENT = 'Interruptions';
const NOCHANGE_INTENT = 'noChange';
const INTERRUPTION_DISPATCHER = 'interruptionDispatcherDialog';
const CONFIRM_CANCEL_PROMPT = 'confirmCancelPrompt';
// LUIS service type entry for turn.n book table LUIS model in the .bot file.
const LUIS_CONFIGURATION = 'cafeBotBookTableTurnN';
module.exports = {
    // This is a custom TextPrompt that uses a LUIS model to handle turn.n conversations including interruptions.
    GetLocationDateTimePartySizePrompt: class extends TextPrompt {
        /**
         * Constructor.
         * @param {String} dialog id
         * @param {BotConfiguration} .bot file configuration
         * @param {StateAccessor} accessor for the reservation property
         * @param {StateAccessor} accessor for on turn property
         * @param {StateAccessor} accessor for user profile property
         */
        constructor(dialogId, botConfig, reservationsAccessor, onTurnAccessor, userProfileAccessor) {
            if (!dialogId) throw new Error('Need dialog ID');
            if (!botConfig) throw new Error('Need bot configuration');
            if (!reservationsAccessor) throw new Error('Need user reservation property accessor');
            if (!onTurnAccessor) throw new Error('Need on turn property accessor');
            // Call super and provide a prompt validator
            super(dialogId, async (turnContext, step) => {
                // validation and prompting logic
                // get reservation property
                let reservationFromState = await this.reservationsAccessor.get(turnContext);
                let newReservation;
                if (reservationFromState === undefined) {
                    newReservation = new Reservation();
                } else {
                    newReservation = Reservation.fromJSON(reservationFromState);
                }
                // if we have a valid reservation, end this prompt.
                //  else get LG based on what's available in reservation property
                if (newReservation.haveCompleteReservation) {
                    if (!newReservation.reservationConfirmed) {
                        if (newReservation.needsChange === true) {
                            await turnContext.sendActivity(`What would you like to change?`);
                        } else {
                            // Greet user with name if we have the user profile set.
                            const userProfile = await this.userProfileAccessor.get(turnContext);
                            if (userProfile !== undefined && userProfile.userName !== '') {
                                await turnContext.sendActivity('Alright ' + userProfile.userName + ', I have a table for ' + newReservation.confirmationReadOut());
                            } else {
                                await turnContext.sendActivity('Ok. I have a table for ' + newReservation.confirmationReadOut());
                            }
                            await turnContext.sendActivity(MessageFactory.suggestedActions(['Yes', 'No'], `Should I go ahead and book the table?`));
                        }
                    } else {
                        step.end(DialogTurnStatus.complete);
                    }
                } else {
                    // readout what has been understood already
                    let groundedPropertiesReadout = newReservation.getGroundedPropertiesReadOut();
                    if (groundedPropertiesReadout !== '') {
                        await turnContext.sendActivity(groundedPropertiesReadout);
                    }
                    // ask user for missing information
                    await turnContext.sendActivity(newReservation.getMissingPropertyReadOut());
                }
            });
            this.reservationsAccessor = reservationsAccessor;
            this.onTurnAccessor = onTurnAccessor;
            this.userProfileAccessor = userProfileAccessor;
            this.qnaDialog = new QnADialog(botConfig, userProfileAccessor);
            // add recognizer
            const luisConfig = botConfig.findServiceByNameOrId(LUIS_CONFIGURATION);
            if (!luisConfig || !luisConfig.appId) throw new Error(`Book Table Turn.N LUIS configuration not found in .bot file. Please ensure you have all required LUIS models created and available in the .bot file. See readme.md for additional information\n`);
            this.luisRecognizer = new LuisRecognizer({
                applicationId: luisConfig.appId,
                azureRegion: luisConfig.region,
                // CAUTION: Its better to assign and use a subscription key instead of authoring key here.
                endpointKey: luisConfig.subscriptionKey
            });
        }
        /**
         * Override dialogContinue.
         *   The override enables
         *     Interruption to be kicked off from right within this dialog.
         *     Ability to leverage a dedicated LUIS model to provide flexible entity filling,
         *     corrections and contextual help.
         *
         * @param {DialogContext} dialog context
         */
        async dialogContinue(dc) {
            let turnContext = dc.context;
            let step = dc.activeDialog.state;
            // get reservation property
            let reservationFromState = await this.reservationsAccessor.get(turnContext);
            let newReservation;
            if (reservationFromState === undefined) {
                newReservation = new Reservation();
            } else {
                newReservation = Reservation.fromJSON(reservationFromState);
            }
            // Get on turn property. This has any entities that mainDispatcher,
            //  or Bot might have captured in its LUIS model
            const onTurnProperties = await this.onTurnAccessor.get(turnContext);
            // if on turn property has entities
            let updateResult;
            if (onTurnProperties !== undefined && onTurnProperties.entities && onTurnProperties.entities.length !== 0) {
                // update reservation property with on turn property results
                updateResult = newReservation.updateProperties(onTurnProperties, step);
            }
            // see if updates to reservation resulted in errors, if so, report them to user.
            if (updateResult &&
                updateResult.status === reservationStatusEnum.INCOMPLETE &&
                updateResult.outcome !== undefined &&
                updateResult.outcome.length !== 0) {
                // set reservation property
                this.reservationsAccessor.set(turnContext, updateResult.newReservation);
                // return and do not continue if there is an error
                await turnContext.sendActivity(updateResult.outcome[0].message);
                return await super.dialogContinue(dc);
            }
            // call LUIS and get results
            const LUISResults = await this.luisRecognizer.recognize(turnContext);
            let topIntent = LuisRecognizer.topIntent(LUISResults);
            // If we dont have an intent match from LUIS, go with the intent available via
            // the on turn property (parent's LUIS model)
            if (Object.keys(LUISResults.intents).length === 0) {
                // go with intent in onTurnProperty
                topIntent = (onTurnProperties.intent || 'None');
            }
            // update object with LUIS result
            updateResult = newReservation.updateProperties(OnTurnProperty.fromLUISResults(LUISResults), step);

            // see if update reservation resulted in errors, if so, report them to user.
            if (updateResult &&
                updateResult.status === reservationStatusEnum.INCOMPLETE &&
                updateResult.outcome !== undefined &&
                updateResult.outcome.length !== 0) {
                // set reservation property
                this.reservationsAccessor.set(turnContext, updateResult.newReservation);
                // return and do not continue if there is an error.
                await turnContext.sendActivity(updateResult.outcome[0].message);
                return await super.dialogContinue(dc);
            }
            // Did user ask for help or said cancel or continuing the conversation?
            switch (topIntent) {
            case CONTINUE_PROMPT_INTENT:
                // user does not want to make any change.
                updateResult.newReservation.needsChange = false;
                break;
            case NOCHANGE_INTENT:
                // user does not want to make any change.
                updateResult.newReservation.needsChange = false;
                break;
            case HELP_INTENT:
                // come back with contextual help
                let helpReadOut = updateResult.newReservation.helpReadOut();
                await turnContext.sendActivity(helpReadOut);
                break;
            case CANCEL_INTENT:
                // start confirmation prompt
                return await dc.prompt(CONFIRM_CANCEL_PROMPT, `Are you sure you want to cancel?`);
            case INTERRUPTIONS_INTENT:
            default:
                // if we picked up new entity values, do not treat this as an interruption
                if (onTurnProperties.entities.length !== 0 || Object.keys(LUISResults.entities).length > 1) break;
                // Handle interruption.
                const onTurnProperty = await this.onTurnAccessor.get(dc.context);
                return await dc.beginDialog(INTERRUPTION_DISPATCHER, onTurnProperty);
            }
            // set reservation property based on OnTurn properties
            this.reservationsAccessor.set(turnContext, updateResult.newReservation);
            return await super.dialogContinue(dc);
        }
        /**
         * Override dialogResume. This is used to handle user's response to confirm cancel prompt.
         *
         * @param {DialogContext} dc
         * @param {DialogReason} reason
         * @param {Object} result
         */
        async dialogResume(dc, reason, result) {
            if (result) {
                // User said yes to cancel prompt.
                await dc.context.sendActivity(`Sure. I've cancelled that!`);
                return await dc.cancelAllDialogs();
            } else {
                // User said no to cancel.
                return await super.dialogResume(dc, reason, result);
            }
        }
    }
};
