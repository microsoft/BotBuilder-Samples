// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
const { DialogTurnStatus, WaterfallDialog, ComponentDialog } = require('botbuilder-dialogs');

const getLocationDateTimePartySizeDialog = require('../shared/dialogs/getLocDateTimePartySizeDialog');
const confirmDialog = require('../shared/dialogs/confirmDialog');
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
     */
    constructor(botConfig, reservationsPropertyAccessor, turnCounterPropertyAccessor, onTurnPropertyAccessor, dialogPropertyAccessor) {
        super(BOOK_TABLE);
        if(!botConfig) throw ('Need bot config');
        if(!reservationsPropertyAccessor) throw ('Need reservations property accessor');
        if(!turnCounterPropertyAccessor) throw ('Need turn counter property accessor');
        if(!onTurnPropertyAccessor) throw ('Need on turn property accessor');
        
        this.reservationsPropertyAccessor = reservationsPropertyAccessor;
        // add dialogs
        this.dialogs = new DialogSet(dialogPropertyAccessor);
        
        this.dialogs.add(new getLocationDateTimePartySizeDialog(botConfig, 
                                                          reservationsPropertyAccessor, 
                                                          turnCounterPropertyAccessor,
                                                          onTurnPropertyAccessor));
        
        this.dialogs.add(new confirmDialog(botConfig, 
                                           reservationsPropertyAccessor, 
                                           turnCounterPropertyAccessor,
                                           onTurnPropertyAccessor));

    }
    async onDialogBegin(dc, options) {
        // Override default begin() logic with bot orchestration logic
        return await this.onDialogContinue(dc);
    }

    async onDialogContinue(dc) {
    }
    
};

BookTableDialog.Name = BOOK_TABLE;

module.exports = BookTableDialog;