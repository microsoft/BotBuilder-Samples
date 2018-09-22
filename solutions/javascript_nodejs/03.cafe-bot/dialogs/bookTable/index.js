// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
const { MessageFactory } = require('botbuilder');
const { ComponentDialog, ConfirmPrompt, DialogTurnStatus, WaterfallDialog } = require('botbuilder-dialogs');
const GetLocDateTimePartySizePrompt = require('../shared/prompts/getLocDateTimePartySize').GetLocationDateTimePartySizePrompt;
const { Reservation } = require('../shared/stateProperties');
const { reservationStatus } = require('../shared/stateProperties/createReservationPropertyResult');
const { InterruptionDispatcher } = require('../dispatcher/interruptionDispatcher');

// This dialog's name. Also matches the name of the intent from ../dispatcher/resources/cafeDispatchModel.lu
// LUIS recognizer replaces spaces ' ' with '_'. So intent name 'Who are you' is recognized as 'Who_are_you'.
const BOOK_TABLE = 'Book_Table';
const BOOK_TABLE_WATERFALL = 'bookTableWaterfall';
const GET_LOCATION_DIALOG_STATE = 'getLocDialogState';
const CONFIRM_DIALOG_STATE = 'confirmDialogState';

const CONFIRM_CANCEL_PROMPT = 'confirmCancelPrompt';

// Turn.N here refers to all back and forth conversations beyond the initial trigger until the book table dialog is completed or cancelled.
const GET_LOCATION_DATE_TIME_PARTY_SIZE_PROMPT = 'getLocationDateTimePartySize';

/**
 * Class Who are you dialog.Uses the same pattern as main dialog and extends ComponentDialog
 */
module.exports = {
    BookTableDialog: class extends ComponentDialog {
        static get Name() { return BOOK_TABLE; }
        /**
         * Constructor.
         *
         * @param {Object} botConfig bot configuration
         * @param {Object} accessor for reservations
         * @param {Object} accessor for on turn
         * @param {Object} accessor for the dialog
         * @param {Object} conversation state object
         */
        constructor(botConfig, reservationsAccessor, onTurnAccessor, userProfileAccessor, conversationState) {
            super(BOOK_TABLE);
            if (!botConfig) throw new Error('Need bot config');
            if (!reservationsAccessor) throw new Error('Need reservations accessor');
            if (!onTurnAccessor) throw new Error('Need on turn accessor');

            this.reservationsAccessor = reservationsAccessor;
            this.onTurnAccessor = onTurnAccessor;
            this.userProfileAccessor = userProfileAccessor;

            // create accessors for child dialogs
            this.getLocDialogAccessor = conversationState.createProperty(GET_LOCATION_DIALOG_STATE);
            this.confirmDialogAccessor = conversationState.createProperty(CONFIRM_DIALOG_STATE);

            // add dialogs
            // Water fall book table dialog
            this.addDialog(new WaterfallDialog(BOOK_TABLE_WATERFALL, [
                this.getAllRequiredProperties.bind(this),
                this.bookTable.bind(this)
            ]));

            // Get location, date, time & party size prompt.
            this.addDialog(new GetLocDateTimePartySizePrompt(GET_LOCATION_DATE_TIME_PARTY_SIZE_PROMPT,
                botConfig,
                reservationsAccessor,
                onTurnAccessor,
                userProfileAccessor));

            // This dialog is interruptable. So add interruptionDispatcherDialog
            this.addDialog(new InterruptionDispatcher(onTurnAccessor, conversationState, userProfileAccessor, botConfig, reservationsAccessor));

            // When user decides to abandon this dialog, we need to confirm user action. Add confirmation prompt
            this.addDialog(new ConfirmPrompt(CONFIRM_CANCEL_PROMPT));
        }

        async getAllRequiredProperties(dc, step) {
            // Get current reservation from accessor
            let reservationFromState = await this.reservationsAccessor.get(dc.context);
            let newReservation;

            if (reservationFromState === undefined) {
                newReservation = new Reservation();
            } else {
                newReservation = Reservation.fromJSON(reservationFromState);
            }
            // Get on turn (includes LUIS entities captured by parent)
            const onTurnProperty = await this.onTurnAccessor.get(dc.context);
            let reservationResult;
            if (onTurnProperty !== undefined) {
                if (newReservation !== undefined) {
                    // update reservation object and gather results.
                    reservationResult = newReservation.updateProperties(onTurnProperty, step);
                } else {
                    reservationResult = Reservation.fromOnTurnProperty(onTurnProperty, step);
                }
            }
            // set reservation
            this.reservationsAccessor.set(dc.context, reservationResult.newReservation);

            // see if update reservation resulted in errors, if so, report them to user.
            if (reservationResult &&
                reservationResult.status === reservationStatus.INCOMPLETE &&
                reservationResult.outcome !== undefined &&
                reservationResult.outcome.length !== 0) {
                // Start the prompt with the initial feedback based on update results.
                return await dc.prompt(GET_LOCATION_DATE_TIME_PARTY_SIZE_PROMPT, reservationResult.outcome[0].message);
            } else {
                if (reservationResult.newReservation.haveCompleteReservation) {
                    await dc.context.sendActivity('Ok. I have a table for ' + reservationResult.newReservation.confirmationReadOut());
                    await dc.context.sendActivity(MessageFactory.suggestedActions(['Yes', 'No'], `Should I go ahead and book the table?`));
                }
                // Start the prompt with the first missing piece of information.
                return await dc.prompt(GET_LOCATION_DATE_TIME_PARTY_SIZE_PROMPT, reservationResult.newReservation.getMissingPropertyReadOut());
            }
        }

        async bookTable(dc, step) {
            // report table booking based on confirmation outcome.
            if (step.result) {
                // User confirmed.
                // Get current reservation from accessor
                let reservationFromState = await this.reservationsAccessor.get(dc.context);
                await dc.context.sendActivity(`Sure. I've booked the table for ` + reservationFromState.confirmationReadOut());
                // TODO: Book the table.
                if (step.result === DialogTurnStatus.complete) {
                    // Clear out the reservation property since this is a successful reservation completion.
                    this.reservationsAccessor.set(dc.context, undefined);
                }
                await dc.cancelAllDialogs();

                return await dc.endDialog();
            } else {
                // User rejected cancellation.
                // clear out state.
                this.reservationsAccessor.set(dc.context, undefined);
                await dc.context.sendActivity(`Ok..I've cancelled the reservation`);
                return await dc.endDialog();
            }
        }
    }
};
