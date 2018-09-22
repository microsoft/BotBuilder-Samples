// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { NumberPrompt } = require('botbuilder-dialogs');

// This is a custom NumberPrompt that requires the value to be between 1 and 99.
module.exports.AgePrompt = class AgePrompt extends NumberPrompt {
    constructor(dialogId) {
        super(dialogId, async (prompt) => {
            if (!prompt.recognized.succeeded) {
                await prompt.context.sendActivity('Please tell me your age!');
                return false;
            } else {
                const value = prompt.recognized.value;
                if (value < 1 || value > 99) {
                    await prompt.context.sendActivity('Please enter an age in years between 1 and 99.');
                    return false;
                } else {
                    return true;
                }
            }
        });
    }
};
