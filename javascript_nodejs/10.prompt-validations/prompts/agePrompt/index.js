const { NumberPrompt } = require('botbuilder-dialogs');

// This is a custom NumberPrompt that requires the value to be between 1 and 99.
module.exports = class AgePrompt extends NumberPrompt {
    constructor(dialogId) {
        super(dialogId, async (context, step) =>{
            if (!step.recognized.succeeded) {
                context.sendActivity('Please tell me your age!');
            } else {
                const value = step.recognized.value;
                if (value < 1 || value > 99) {
                    context.sendActivity('Please enter an age in years between 1 and 99');
                } else {
                    step.end(value);
                }
            }
        });
    }
}