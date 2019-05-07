// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

import { TimexProperty } from '@microsoft/recognizers-text-data-types-timex-expression';
import { DateTimePrompt, DateTimeResolution, DialogTurnResult, PromptValidatorContext, WaterfallDialog, WaterfallStepContext } from 'botbuilder-dialogs';
import { CancelAndHelpDialog } from './cancelAndHelpDialog';

const DATETIME_PROMPT = 'datetimePrompt';
const WATERFALL_DIALOG = 'waterfallDialog';

export class DateResolverDialog extends CancelAndHelpDialog {
    constructor(id: string) {
        super(id || 'dateResolverDialog');
        this.addDialog(new DateTimePrompt(DATETIME_PROMPT, this.dateTimePromptValidator.bind(this)))
            .addDialog(new WaterfallDialog(WATERFALL_DIALOG, [
                this.initialStep.bind(this),
                this.finalStep.bind(this),
            ]));

        this.initialDialogId = WATERFALL_DIALOG;
    }

    private async initialStep(stepContext: WaterfallStepContext): Promise<DialogTurnResult> {
        const timex = (stepContext.options as any).date;

        const promptMsg = 'On what date would you like to travel?';
        const repromptMsg = 'I\'m sorry, for best results, please enter your travel date including the month, day and year.';

        if (!timex) {
            // We were not given any date at all so prompt the user.
            return await stepContext.prompt(DATETIME_PROMPT,
                {
                    prompt: promptMsg,
                    retryPrompt: repromptMsg,
                });
        } else {
            // We have a Date we just need to check it is unambiguous.
            const timexProperty = new TimexProperty(timex);
            if (!timexProperty.types.has('definite')) {
                // This is essentially a "reprompt" of the data we were given up front.
                return await stepContext.prompt(DATETIME_PROMPT, { prompt: repromptMsg });
            } else {
                return await stepContext.next({ timex });
            }
        }
    }

    private async finalStep(stepContext: WaterfallStepContext): Promise<DialogTurnResult> {
        const timex = stepContext.result[0].timex;
        return await stepContext.endDialog(timex);
    }

    private async dateTimePromptValidator(promptContext: PromptValidatorContext<DateTimeResolution>): Promise<boolean> {
        if (promptContext.recognized.succeeded) {
            // This value will be a TIMEX. And we are only interested in a Date so grab the first result and drop the Time part.
            // TIMEX is a format that represents DateTime expressions that include some ambiguity. e.g. missing a Year.
            const timex = promptContext.recognized.value[0].timex.split('T')[0];

            // If this is a definite Date including year, month and day we are good otherwise reprompt.
            // A better solution might be to let the user know what part is actually missing.
            return new TimexProperty(timex).types.has('definite');
        } else {
            return false;
        }
    }
}
