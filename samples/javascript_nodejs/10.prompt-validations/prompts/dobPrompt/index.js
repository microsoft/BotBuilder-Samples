// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { DateTimePrompt } = require('botbuilder-dialogs');

const DATE_LOW_BOUNDS = new Date('1918-08-24');
const DATE_HIGH_BOUNDS = new Date('2018-08-24');

// This is a custom DateTimePrompt that requires the date to be between DATE_LOW_BOUNDS and DATE_HIGH_BOUNDS.
// This prompt is based on a powerful library of entity recognizers that can do much more than pull
// a date from a string, and can help with input like "How about tonight at 8". Learn more below:
// https://github.com/Microsoft/Recognizers-Text/tree/master/JavaScript/packages/recognizers-text-suite
module.exports.DobPrompt = class DobPrompt extends DateTimePrompt {
    constructor(dialogId) {
        super(dialogId, async (prompt) => {
            try {
                if (!prompt.recognized.succeeded) {
                    throw new Error('No date found.');
                }
                const values = prompt.recognized.value;
                if (!Array.isArray(values) || values.length < 0) { throw new Error('Missing time.'); }
                if ((values[0].type !== 'datetime') && (values[0].type !== 'date')) { throw new Error('Unsupported type.'); }
                const value = new Date(values[0].value);
                if (value.getTime() < DATE_LOW_BOUNDS.getTime()) {
                    throw new Error('Date too low.');
                } else if (value.getTime() > DATE_HIGH_BOUNDS.getTime()) {
                    throw new Error('Date too high.');
                }
                return true;
            } catch (err) {
                await prompt.context.sendActivity(`Answer with a date like 1978-01-25 or say "cancel".`);
                return false;
            }
        });
    }
};
