// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { InputHints, MessageFactory } = require('botbuilder');
const { DateTimePrompt, WaterfallDialog } = require('botbuilder-dialogs');
const { CancelAndHelpDialog } = require('./cancelAndHelpDialog');
const { TimexProperty } = require('@microsoft/recognizers-text-data-types-timex-expression');

const DATETIME_PROMPT = 'datetimePrompt';
const WATERFALL_DIALOG = 'waterfallDialog';

class DateResolverDialog extends CancelAndHelpDialog {
    constructor(id) {
        super(id || 'dateResolverDialog');
        this.addDialog(new DateTimePrompt(DATETIME_PROMPT, this.dateTimePromptValidator.bind(this)))
            .addDialog(new WaterfallDialog(WATERFALL_DIALOG, [
                this.initialStep.bind(this),
                this.finalStep.bind(this)
            ]));

        this.initialDialogId = WATERFALL_DIALOG;
    }

    async initialStep(stepContext) {
        const timex = stepContext.options.date;

        const promptMessageText = 'On what date would you like to travel?';
        const promptMessage = MessageFactory.text(promptMessageText, promptMessageText, InputHints.ExpectingInput);

        const repromptMessageText = "I'm sorry, for best results, please enter your travel date including the month, day and year.";
        const repromptMessage = MessageFactory.text(repromptMessageText, repromptMessageText, InputHints.ExpectingInput);

        if (!timex) {
            // We were not given any date at all so prompt the user.
            return await stepContext.prompt(DATETIME_PROMPT,
                {
                    prompt: promptMessage,
                    retryPrompt: repromptMessage
                });
        }
        // We have a Date we just need to check it is unambiguous.
        const timexProperty = new TimexProperty(timex);
        if (!timexProperty.types.has('definite')) {
            // This is essentially a "reprompt" of the data we were given up front.
            return await stepContext.prompt(DATETIME_PROMPT, { prompt: repromptMessage });
        }
        return await stepContext.next([{ timex: timex }]);
    }

    async finalStep(stepContext) {
        const timex = stepContext.result[0].timex;
        return await stepContext.endDialog(timex);
    }

    async dateTimePromptValidator(promptContext) {
        if (promptContext.recognized.succeeded) {
            // This value will be a TIMEX. And we are only interested in a Date so grab the first result and drop the Time part.
            // TIMEX is a format that represents DateTime expressions that include some ambiguity. e.g. missing a Year.
            const timex = promptContext.recognized.value[0].timex.split('T')[0];

            // If this is a definite Date including year, month and day we are good otherwise reprompt.
            // A better solution might be to let the user know what part is actually missing.
            return new TimexProperty(timex).types.has('definite');
        }
        return false;
    }
}

module.exports.DateResolverDialog = DateResolverDialog;
