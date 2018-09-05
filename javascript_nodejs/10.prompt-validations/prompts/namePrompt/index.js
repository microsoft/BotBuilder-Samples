const { TextPrompt } = require('botbuilder-dialogs');


// This is a custom TextPrompt that requires the input to be between 1 and 50 characters in length.
module.exports = class NamePrompt extends TextPrompt {
    constructor(dialogId) {
        super(dialogId, async (context, step) => {
            if (!step.recognized) {
                context.sendActivity('Please tell me your name!');
            } else {
                const value = step.recognized.value;
                if (value.length < 1) {
                    context.sendActivity('Your name has to include at least one character.');
                } else if (value.length > 50) {
                    context.sendActivity(`Sorry, but I can only handle names of up to 50 characters.  Yours was ${ value.length }.`);
                } else {
                    step.end(value);
                }
            }
        });
    }
}