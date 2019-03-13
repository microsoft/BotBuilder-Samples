// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { TimexProperty } = require('@microsoft/recognizers-text-data-types-timex-expression');
const { ConfirmPrompt, TextPrompt, WaterfallDialog } = require('botbuilder-dialogs');
const { CancelAndHelpDialog } = require('./cancelAndHelpDialog');
const { DateResolverDialog } = require('./dateResolverDialog');

const CONFIRM_PROMPT = 'confirmPrompt';
const DATE_RESOLVER_DIALOG = 'dateResolverDialog';
const TEXT_PROMPT = 'textPrompt';
const WATERFALL_DIALOG = 'waterfallDialog';

class BookingDialog extends CancelAndHelpDialog {
    constructor(id) {
        super(id || 'bookingDialog');

        this.addDialog(new TextPrompt(TEXT_PROMPT))
            .addDialog(new ConfirmPrompt(CONFIRM_PROMPT))
            .addDialog(new DateResolverDialog(DATE_RESOLVER_DIALOG))
            .addDialog(new WaterfallDialog(WATERFALL_DIALOG, [
                this.destinationStep.bind(this),
                this.originStep.bind(this),
                this.travelDateStep.bind(this),
                this.confirmStep.bind(this),
                this.finalStep.bind(this)
            ]));

        this.initialDialogId = WATERFALL_DIALOG;
    }

    async destinationStep(stepContext) {
        const bookingDetails = stepContext.options;

        if (!bookingDetails.destination) {
            return await stepContext.prompt(TEXT_PROMPT, { prompt: 'Where would you like to travel to?' });
        } else {
            return await stepContext.next(bookingDetails.destination);
        }
    }
    
    async originStep(stepContext) {
        const bookingDetails = stepContext.options;

        bookingDetails.destination = stepContext.result;
        if (!bookingDetails.origin) {
            return await stepContext.prompt(TEXT_PROMPT, { prompt: 'Where are you traveling from?' });
        } else {
            return await stepContext.next(bookingDetails.origin);
        }
    }

    async travelDateStep(stepContext) {
        const bookingDetails = stepContext.options;

        bookingDetails.origin = stepContext.result;
        if (!bookingDetails.travelDate || this.isAmbiguous(bookingDetails.travelDate)) {
            return await stepContext.beginDialog(DATE_RESOLVER_DIALOG, bookingDetails.travelDate);
        } else {
            return await stepContext.next(bookingDetails.travelDate);
        }
    }

    async confirmStep(stepContext) {
        const bookingDetails = stepContext.options;

        bookingDetails.travelDate = stepContext.result;
        const msg = `Please confirm, I have you traveling to: ${bookingDetails.destination} from: ${bookingDetails.origin} on: ${bookingDetails.travelDate}`;

        return await stepContext.prompt(CONFIRM_PROMPT, { prompt: msg });
    }

    async finalStep(stepContext) {
        if (stepContext.result == true) {
            const bookingDetails = stepContext.options;

            return await stepContext.endDialog(bookingDetails);
        } else {
            return await stepContext.endDialog();
        }
    }

    isAmbiguous(timex) {
        const timexPropery = new TimexProperty(timex);
        return !timexPropery.types.has('definite');
    }
}

module.exports.BookingDialog = BookingDialog;
