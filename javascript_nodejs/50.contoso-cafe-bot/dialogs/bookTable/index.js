// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
const { DialogTurnStatus, WaterfallDialog, ComponentDialog } = require('botbuilder-dialogs');

const getLocationDateTimePartySize = require('./getLocationDateTimePartySize');
const onTurnProperty = require('../shared/stateProperties/onTurnProperty');
const turnResult = require('../shared/turnResult');
const ReservationProperty = require('../shared/stateProperties/reservationProperty');
// This dialog's name. Also matches the name of the intent from ../mainDialog/resources/cafeDispatchModel.lu
// LUIS recognizer replaces spaces ' ' with '_'. So intent name 'Who are you' is recognized as 'Who_are_you'.
const BOOK_TABLE = 'Book_Table';

const DIALOG_START = 'Start';
// Turn.N here refers to all back and forth conversations beyond the initial trigger until the book table dialog is completed or cancelled.
const GET_BOOK_TABLE_TURN_N = 'bookTableTurnN';

/**
 * Class Who are you dialog.
 */
class BookTableDialog extends ComponentDialog {
    /**
     * Constructor.
     * 
     * @param {Object} botConfig bot configuration
     * @param {Object} accessor for reservations property
     * @param {Object} accessor for turn counter property
     * @param {Object} accessor for on turn property
     */
    constructor(botConfig, reservationsPropertyAccessor, turnCounterPropertyAccessor, onTurnPropertyAccessor) {
        super(BOOK_TABLE);
        if(!botConfig) throw ('Need bot config');
        if(!reservationsPropertyAccessor) throw ('Need reservations property accessor');
        if(!turnCounterPropertyAccessor) throw ('Need turn counter property accessor');
        if(!onTurnPropertyAccessor) throw ('Need on turn property accessor');
        
        this.reservationsPropertyAccessor = reservationsPropertyAccessor;

        // add dialogs
        this.addDialog(new WaterfallDialog(DIALOG_START, [
            this.getLocationDateTimePartySize,
            this.bookTable
        ]));

        // Helper sub-dialog that captures all required information to book a table
        this.addDialog(new getLocationDateTimePartySize(GET_BOOK_TABLE_TURN_N, 
                                                        botConfig, 
                                                        reservationsPropertyAccessor, 
                                                        turnCounterPropertyAccessor,
                                                        onTurnPropertyAccessor));
    }
    /**
     * Waterfall step to resolve all required information to make a reservation including location, date, time, party size. 
     * 
     * @param {Object} dc Dialog context
     * @param {Object} step Dialog turn result
     */
    async getLocationDateTimePartySize(dc, step) {
        // This one waterfall step is executed until all required information is captured.
        let turnResult = dc.continue();

        if(turnResult.status === DialogTurnStatus.empty) {
            dc.begin(GET_BOOK_TABLE_TURN_N); // TODO: pass in step context passed in? 
        } // TODO: might need to handle other types like interruption and abandon
        return turnResult;
    }
    /**
     * Waterfall step to finalize user's response and greet user.
     * 
     * @param {Object} dc Dialog context
     * @param {Object} step Dialog turn result
     */
    async bookTable(dc, step) {
        // read from reservations property accessor
        let newReservation = await this.reservationsPropertyAccessor.get(dc.context);

        // Handle interruption.
        // if(step.result.reason && step.result.reason === 'Interruption') {
        //     // set onTurnProperty in the payload so this can be resumed back if needed by main dialog.
        //     if(step.result.payload === undefined) {
        //         step.result.payload = {onTurnProperty: new onTurnProperty(WHO_ARE_YOU)};
        //     }
        //     else {
        //         step.result.payload.onTurnProperty = new onTurnProperty(WHO_ARE_YOU);
        //     }
        //     return new turnResult(DialogTurnStatus.empty, step.result);
        // }
        return await dc.end();
    }
};

BookTableDialog.Name = BOOK_TABLE;

module.exports = BookTableDialog;