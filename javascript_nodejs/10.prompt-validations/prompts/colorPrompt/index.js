// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ChoicePrompt } = require('botbuilder-dialogs');

// This is a custom choice prompt that will emit an error if the user
// types an invalid choice.
module.exports = class ColorPrompt extends ChoicePrompt {
    constructor(dialogId) {
        super(dialogId, async (turnContext, step) => {
            if (!step.recognized.succeeded) {
                // An invalid choice was received, emit an error.
                await turnContext.sendActivity(`Sorry, "${ turnContext.activity.text }" is not on my list.`);
            } else {
                step.end(step.recognized.value.value);
            }
        });
    }
}