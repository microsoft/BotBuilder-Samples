// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { TextPrompt } = require('botbuilder-dialogs');

// This is a custom TextPrompt that requires the input to be between 1 and 50 characters in length.
module.exports.NamePrompt = class NamePrompt extends TextPrompt {
    constructor(dialogId) {
        super(dialogId, async (prompt) => {
            if (!prompt.recognized.succeeded) {
                await prompt.context.sendActivity('Please tell me your name.');
                return false;
            } else {
                const value = prompt.recognized.value;
                if (value.length < 1) {
                    await prompt.context.sendActivity('Your name has to include at least one character.');
                    return false;
                } else if (value.length > 50) {
                    await prompt.context.sendActivity(`Sorry, but I can only handle names of up to 50 characters. Yours was ${ value.length }.`);
                    return false;
                } else {
                    return true;
                }
            }
        });
    }
};
