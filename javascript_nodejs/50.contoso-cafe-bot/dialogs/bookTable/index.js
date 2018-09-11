// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
const { DialogTurnStatus, WaterfallDialog, ComponentDialog, DialogSet, DateTimePrompt } = require('botbuilder-dialogs');

const getLocationDateTimePartySizePrompt = require('../shared/prompts/getLocDateTimePartySize');
const confirmDialog = require('../shared/dialogs/confirmDialog');
const { onTurnProperty, reservationProperty } = require('../shared/stateProperties');
const turnResult = require('../shared/turnResult');

// This dialog's name. Also matches the name of the intent from ../mainDialog/resources/cafeDispatchModel.lu
// LUIS recognizer replaces spaces ' ' with '_'. So intent name 'Who are you' is recognized as 'Who_are_you'.
const BOOK_TABLE = 'Book_Table';
const BOOK_TABLE_DIALOG_STATE = 'bookTable';
const GET_LOCATION_DIALOG_STATE = 'getLocDialogState';
const CONFIRM_DIALOG_STATE = 'confirmDialogState';

const DIALOG_START = 'Start';

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
    constructor(botConfig, reservationsPropertyAccessor, turnCounterPropertyAccessor, onTurnPropertyAccessor, dialogPropertyAccessor, conversationState) {
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
        
        // add dialogs
        this.dialogs = new DialogSet(this.bookTableDialogPropertyAccessor);

        // Get location, date, time & party size prompt.
        this.addDialog(new getLocationDateTimePartySizePrompt(GET_LOCATION_DATE_TIME_PARTY_SIZE_PROMPT,
                                                              botConfig, 
                                                              reservationsPropertyAccessor, 
                                                              onTurnPropertyAccessor));
        
        // Confirm dialog.
        this.addDialog(new confirmDialog(botConfig, 
                                           reservationsPropertyAccessor, 
                                           turnCounterPropertyAccessor,
                                           onTurnPropertyAccessor));
        
    }
    async onDialogBegin(dc, options) {
        // Override default begin() logic with bot orchestration logic
        return await this.onDialogContinue(dc);
    }

    async onDialogContinue(dc) {
        // Call active dialog and get results
        let turnResults = await dc.continue();

        if(turnResults.status === DialogTurnStatus.empty) {
            // Begin the right dialog based on what we have in reservation
            const newReservation = await this.reservationsPropertyAccessor.get(dc.context);
            if(newReservation === undefined || !newReservation.haveCompleteReservationProperty()) {
                return await dc.begin(GET_LOCATION_DATE_TIME_PARTY_SIZE_PROMPT);
            } else {
                // Move on to confirm the reservation.
                return await dc.begin(confirmDialog.Name)
            }
        } else {
            // handle interruptions.
        }
        
    }
    
};

BookTableDialog.Name = BOOK_TABLE;

module.exports = BookTableDialog;