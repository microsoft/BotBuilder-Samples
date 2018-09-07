// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { NumberPrompt } = require('botbuilder-dialogs');

// This is a custom NumberPrompt that requires the value to be between 1 and 99.
module.exports = class AgePrompt extends NumberPrompt {
    constructor(dialogId) {
        super(dialogId, async (turnContext, step) =>{
            if (!step.recognized.succeeded) {
                await turnContext.sendActivity('Please tell me your age!');
            } else {
                const value = step.recognized.value;
                if (value < 1 || value > 99) {
                    await turnContext.sendActivity('Please enter an age in years between 1 and 99.');
                } else {
                    step.end(value);
                }
            }
        });
    }
}