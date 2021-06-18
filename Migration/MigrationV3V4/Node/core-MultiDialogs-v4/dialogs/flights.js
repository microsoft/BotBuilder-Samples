// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ComponentDialog, WaterfallDialog } = require('botbuilder-dialogs');

const initialId = 'flightsWaterfallDialog';

class FlightDialog extends ComponentDialog {
    constructor(id) {
        super(id);

        // ID of the child dialog that should be started anytime the component is started.
        this.initialDialogId = initialId;

        // Define the conversation flow using a waterfall model.
        this.addDialog(new WaterfallDialog(initialId, [
            async () => {
                throw new Error('Flights Dialog is not implemented and is instead ' +
                    'being used to show Bot error handling');
            }
        ]));
    }
}

exports.FlightDialog = FlightDialog;
