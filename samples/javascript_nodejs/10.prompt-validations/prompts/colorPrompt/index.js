// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ChoicePrompt } = require('botbuilder-dialogs');

// This is a custom choice prompt that will emit an error if the user
// types an invalid choice.
module.exports.ColorPrompt = class ColorPrompt extends ChoicePrompt {
    constructor(dialogId) {
        super(dialogId, async (prompt) => {
            if (!prompt.recognized.succeeded) {
                // An invalid choice was received, emit an error.
                await prompt.context.sendActivity(`Sorry, "${ prompt.context.activity.text }" is not on my list.`);
                return false;
            } else {
                return true;
            }
        });
    }
};
