// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
const { DialogTurnStatus, WaterfallDialog, ComponentDialog, DialogSet, DateTimePrompt, ConfirmPrompt } = require('botbuilder-dialogs');

const getLocationDateTimePartySizePrompt = require('../shared/prompts/getLocDateTimePartySize');
const { onTurnProperty, reservationProperty } = require('../shared/stateProperties');

// This dialog's name. Also matches the name of the intent from ../mainDialog/resources/cafeDispatchModel.lu
// LUIS recognizer replaces spaces ' ' with '_'. So intent name 'Who are you' is recognized as 'Who_are_you'.
const BOOK_TABLE = 'Book_Table';
const BOOK_TABLE_WATERFALL = 'bookTableWaterfall'
const BOOK_TABLE_DIALOG_STATE = 'bookTable';
const GET_LOCATION_DIALOG_STATE = 'getLocDialogState';
const CONFIRM_DIALOG_STATE = 'confirmDialogState';
const CONFIRM_RESERVATION_PROMPT = 'confirmReservation';
const DIALOG_START = 'Start';
const { ReservationOutcome, ReservationResult, reservationStatus } = require('../shared/stateProperties/createReservationPropertyResult');

// Turn.N here refers to all back and forth conversations beyond the initial trigger until the book table dialog is completed or cancelled.
const GET_LOCATION_DATE_TIME_PARTY_SIZE_PROMPT = 'getLocationDateTimePartySize';

/**
 * Class Who are you dialog.Uses the same pattern as main dialog and extends ComponentDialog
 */
class BookTableDialog extends ComponentDialog {
    /**
     * Constructor.
     * 
     * @param {Object} botConfig bot configuration
     * @param {Object} accessor for reservations property
     * @param {Object} accessor for turn counter property
     * @param {Object} accessor for on turn property
     * @param {Object} accessor for the dialog property
     * @param {Object} conversation state object
     */
    constructor(botConfig, reservationsPropertyAccessor, turnCounterPropertyAccessor, onTurnPropertyAccessor, userProfilePropertyAccessor, conversationState) {
        super(BOOK_TABLE);
        if(!botConfig) throw ('Need bot config');
        if(!reservationsPropertyAccessor) throw ('Need reservations property accessor');
        if(!turnCounterPropertyAccessor) throw ('Need turn counter property accessor');
        if(!onTurnPropertyAccessor) throw ('Need on turn property accessor');
        
        this.reservationsPropertyAccessor = reservationsPropertyAccessor;
        this.onTurnPropertyAccessor = onTurnPropertyAccessor;

        // create property accessors
        this.bookTableDialogPropertyAccessor = conversationState.createProperty(BOOK_TABLE_DIALOG_STATE);
        // create property accessors for child dialogs
        this.getLocDialogPropertyAccessor = conversationState.createProperty(GET_LOCATION_DIALOG_STATE);
        this.confirmDialogPropertyAccessor = conversationState.createProperty(CONFIRM_DIALOG_STATE);
        this.userProfilePropertyAccessor = userProfilePropertyAccessor;
        
        // add dialogs
        this.dialogs = new DialogSet(this.bookTableDialogPropertyAccessor);

        // Water fall dialog
        this.addDialog(new WaterfallDialog(BOOK_TABLE_WATERFALL, [
            this.getAllRequiredProperties.bind(this),
            this.confirmReservation.bind(this),
            this.bookTable.bind(this)
        ]));

        // Get location, date, time & party size prompt.
        this.addDialog(new getLocationDateTimePartySizePrompt(GET_LOCATION_DATE_TIME_PARTY_SIZE_PROMPT,
                                                              botConfig, 
                                                              reservationsPropertyAccessor, 
                                                              onTurnPropertyAccessor, 
                                                              userProfilePropertyAccessor));
        
        // Confirm prompt.
        this.addDialog(new ConfirmPrompt(CONFIRM_RESERVATION_PROMPT));
    }
    
    async getAllRequiredProperties(dc, step) {
        // Get current reservation property from accessor
        let reservationFromState = await this.reservationsPropertyAccessor.get(dc.context);
        let newReservation; 

        if(reservationFromState === undefined) {
            newReservation = new reservationProperty(); 
        } else {
            newReservation = reservationProperty.fromJSON(reservationFromState);
        }
        // Get on turn property (includes LUIS entities captured by parent)
        const onTurnProperty = await this.onTurnPropertyAccessor.get(dc.context);
        let reservationResult;
        if(onTurnProperty !== undefined) {
            if(newReservation !== undefined) {
                // update reservation object and gather results.
                reservationResult = newReservation.updateProperties(onTurnProperty);
            } else {
                reservationResult = reservationProperty.fromOnTurnProperty(onTurnProperty);
            }
        }
        // set reservation property 
        this.reservationsPropertyAccessor.set(dc.context, reservationResult.newReservation);
        
        // see if updadte reservtion resulted in errors, if so, report them to user. 
        if(reservationResult &&
            reservationResult.status === reservationStatus.INCOMPLETE &&
            reservationResult.outcome !== undefined &&
            reservationResult.outcome.length !== 0) {
                // Start the prompt with the initial feedback based on update results.
                return await dc.prompt(GET_LOCATION_DATE_TIME_PARTY_SIZE_PROMPT, reservationResult.outcome[0].message);
        } else {
            // Start the prompt with the first missing piece of information. 
            return await dc.prompt(GET_LOCATION_DATE_TIME_PARTY_SIZE_PROMPT, reservationResult.newReservation.getMissingPropertyReadOut());
        }
    }

    async confirmReservation(dc, step) {
        // Get current reservation property from accessor
        let reservationFromState = await this.reservationsPropertyAccessor.get(dc.context);
        let newReservation; 

        if(reservationFromState === undefined) {
            newReservation = new reservationProperty(); 
        } else {
            newReservation = reservationProperty.fromJSON(reservationFromState);
        }
        await dc.context.sendActivity(newReservation.confirmationReadOut());
        // prompt for confirmation to cancel
        return await dc.prompt(CONFIRM_RESERVATION_PROMPT, `Should I go ahead and book the table?`);
    }

    async bookTable(dc, step) {
        // report table booking based on confirmation outcome.
        if (step.result) {
            // User confirmed.
            // TODO: Book the table.
            // clear out state
            this.reservationsPropertyAccessor.set(dc.context, undefined);
            await dc.cancelAll();
            await dc.context.sendActivity(`Sure. I've booked the table`);
            return await dc.end();
        } else {
            // User rejected cancellation.
            // clear out state.
            this.reservationsPropertyAccessor.set(dc.context, undefined);
            await dc.context.sendActivity(`Ok..I've cancelled the reservation`);
            return await dc.end();
        }
    }    
};

BookTableDialog.Name = BOOK_TABLE;

module.exports = BookTableDialog;