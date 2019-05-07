// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { TimexProperty } from '@microsoft/recognizers-text-data-types-timex-expression';
import {
    ConfirmPrompt,
    DialogTurnResult,
    TextPrompt,
    WaterfallDialog,
    WaterfallStepContext,
} from 'botbuilder-dialogs';
import { BookingDetails } from './bookingDetails';
import { CancelAndHelpDialog } from './cancelAndHelpDialog';
import { DateResolverDialog } from './dateResolverDialog';

const CONFIRM_PROMPT = 'confirmPrompt';
const DATE_RESOLVER_DIALOG = 'dateResolverDialog';
const TEXT_PROMPT = 'textPrompt';
const WATERFALL_DIALOG = 'waterfallDialog';

export class BookingDialog extends CancelAndHelpDialog {
    constructor(id: string) {
        super(id || 'bookingDialog');

        this.addDialog(new TextPrompt(TEXT_PROMPT))
            .addDialog(new ConfirmPrompt(CONFIRM_PROMPT))
            .addDialog(new DateResolverDialog(DATE_RESOLVER_DIALOG))
            .addDialog(new WaterfallDialog(WATERFALL_DIALOG, [
                this.destinationStep.bind(this),
                this.originStep.bind(this),
                this.travelDateStep.bind(this),
                this.confirmStep.bind(this),
                this.finalStep.bind(this),
            ]));

        this.initialDialogId = WATERFALL_DIALOG;
    }

    /**
     * If a destination city has not been provided, prompt for one.
     */
    private async destinationStep(stepContext: WaterfallStepContext): Promise<DialogTurnResult> {
        const bookingDetails = stepContext.options as BookingDetails;

        if (!bookingDetails.destination) {
            return await stepContext.prompt(TEXT_PROMPT, { prompt: 'To what city would you like to travel?' });
        } else {
            return await stepContext.next(bookingDetails.destination);
        }
    }

    /**
     * If an origin city has not been provided, prompt for one.
     */
    private async originStep(stepContext: WaterfallStepContext): Promise<DialogTurnResult> {
        const bookingDetails = stepContext.options as BookingDetails;

        // Capture the response to the previous step's prompt
        bookingDetails.destination = stepContext.result;
        if (!bookingDetails.origin) {
            return await stepContext.prompt(TEXT_PROMPT, { prompt: 'From what city will you be travelling?' });
        } else {
            return await stepContext.next(bookingDetails.origin);
        }
    }

    /**
     * If a travel date has not been provided, prompt for one.
     * This will use the DATE_RESOLVER_DIALOG.
     */
    private async travelDateStep(stepContext: WaterfallStepContext): Promise<DialogTurnResult> {
        const bookingDetails = stepContext.options as BookingDetails;

        // Capture the results of the previous step
        bookingDetails.origin = stepContext.result;
        if (!bookingDetails.travelDate || this.isAmbiguous(bookingDetails.travelDate)) {
            return await stepContext.beginDialog(DATE_RESOLVER_DIALOG, { date: bookingDetails.travelDate });
        } else {
            return await stepContext.next(bookingDetails.travelDate);
        }
    }

    /**
     * Confirm the information the user has provided.
     */
    private async confirmStep(stepContext: WaterfallStepContext): Promise<DialogTurnResult> {
        const bookingDetails = stepContext.options as BookingDetails;

        // Capture the results of the previous step
        bookingDetails.travelDate = stepContext.result;
        const msg = `Please confirm, I have you traveling to: ${ bookingDetails.destination } from: ${ bookingDetails.origin } on: ${ bookingDetails.travelDate }.`;

        // Offer a YES/NO prompt.
        return await stepContext.prompt(CONFIRM_PROMPT, { prompt: msg });
    }

    /**
     * Complete the interaction and end the dialog.
     */
    private async finalStep(stepContext: WaterfallStepContext): Promise<DialogTurnResult> {
        if (stepContext.result === true) {
            const bookingDetails = stepContext.options as BookingDetails;

            return await stepContext.endDialog(bookingDetails);
        } else {
            return await stepContext.endDialog();
        }
    }

    private isAmbiguous(timex: string): boolean {
        const timexPropery = new TimexProperty(timex);
        return !timexPropery.types.has('definite');
    }
}
