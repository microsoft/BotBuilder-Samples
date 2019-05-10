const { ComponentDialog, WaterfallDialog } = require('botbuilder-dialogs');
const { 
    FLIGHTS_WATERFALL_DIALOG
} = require('../const');

const initialId = FLIGHTS_WATERFALL_DIALOG;

class FlightDialog extends ComponentDialog {
    constructor(id) {
        super(id);

        // ID of the child dialog that should be started anytime the component is started.
        this.initialDialogId = initialId;

        // Define the conversation flow using a waterfall model.
        this.addDialog(new WaterfallDialog(initialId, [
            async function (step) {
                await step.context.sendActivity('Flights Dialog is not implemented and is instead being used to show Bot error handling');
                // End the dialog
                return await step.endDialog();
            }
        ]));
    }
}

exports.FlightDialog = FlightDialog;