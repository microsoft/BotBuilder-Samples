const { ChoicePrompt } = require('botbuilder-dialogs');

module.exports = class ColorPrompt extends ChoicePrompt {
    constructor(dialogId) {
        super(dialogId, async (context, step) => {
            if (!step.recognized.succeeded) {
                // an invalid choice was received, emit an error.
                context.sendActivity(`Sorry, "${ context.activity.text }" is not on my list.`);
            } else {
                step.end(step.recognized.value.value);
            }
        });
    }
}