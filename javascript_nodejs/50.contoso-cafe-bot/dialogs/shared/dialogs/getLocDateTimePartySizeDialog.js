// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
const { DialogTurnStatus, WaterfallDialog, ComponentDialog } = require('botbuilder-dialogs');

// Dialog name from ../../bookTable/resources/turn-N.lu
const DIALOG_NAME = 'GetLocationDateTimePartySize';

class GetLocDateTimePartySize extends ComponentDialog {
    constructor() {
        super(DIALOG_NAME);

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
        // If getLocationDateTimePartySize completed successfully, 
            // read from reservations property accessor
            let newReservation = await this.reservationsPropertyAccessor.get(dc.context);

            // Attempt to book table.
            // TODO: add code to book table.

            // Report outcome
        // else, handle interruptions and other outcomes
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
GetLocDateTimePartySize.Name = DIALOG_NAME;
module.exports = GetLocDateTimePartySize;