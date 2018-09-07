// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { DateTimePrompt } = require('botbuilder-dialogs');

const DATE_LOW_BOUNDS = new Date('8/24/1918');
const DATE_HIGH_BOUNDS = new Date('8/24/2018');

// This is a custom DateTimePrompt that requires the date to be between DATE_LOW_BOUNDS and DATE_HIGH_BOUNDS.
module.exports = class DOBPrompt extends DateTimePrompt {
    constructor(dialogId) {
        super(dialogId, async (turnContext, step) => {
            try {
                if (!step.recognized.succeeded) {
                    throw new Error('no date found');
                }
                const values = step.recognized.value;
                if (!Array.isArray(values) || values.length < 0) { throw new Error('missing time') }
                if ((values[0].type !== 'datetime') && (values[0].type !== 'date')) { throw new Error('unsupported type') }
                const value = new Date(values[0].value);
                if (value.getTime() < DATE_LOW_BOUNDS.getTime()) {
                    throw new Error('too low');
                } else if (value.getTime() > DATE_HIGH_BOUNDS.getTime()) {
                    throw new Error('too high');
                }
                step.end(value);
            } catch (err) {
                await turnContext.sendActivity(`Answer with a date like 8/8/2018 or say "cancel".`);
            }
        });
    }
}