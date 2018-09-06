const { Dialog, DialogReason } = require('botbuilder-dialogs');

class ScriptedDialog extends Dialog {

    constructor (dialogId, path_to_json, onComplete) {

        super(dialogId);

        try {
            this.script = require(path_to_json);
        } catch(err) {
            throw new Error(err);
        }

        if (onComplete) {
            this.onComplete = onComplete;
        }

    }

    async dialogBegin(dc, options) {
        // Initialize the state
        const state = dc.activeDialog.state;
        state.options = options || {};
        state.values = {};

        // Run the first step
        return await this.runStep(dc, 0, DialogReason.beginCalled);
    }

    async dialogContinue(dc) {
        // Don't do anything for non-message activities
        if (dc.context.activity.type !== ActivityTypes.Message) {
            return Dialog.EndOfTurn;
        }

        // Run next step with the message text as the result.
        return await this.dialogResume(dc, DialogReason.continueCalled, dc.context.activity.text);
    }

    async dialogResume(dc, reason, result) {
        // Increment step index and run step
        const state = dc.activeDialog.state;

        return await this.runStep(dc, state.stepIndex + 1, reason, result);
    }

    async onStep(dc, step) {

        // Let's interpret the current line of the script.
        var line = this.script[step.index];

        var previous = (step.index >= 1) ? this.script[step.index - 1] : null;

        // Capture the previous step value if there previous line included a prompt
        if (previous && previous.prompt) {
            if (previous.prompt.responseKey) {
                step.values[previous.prompt.responseKey] = step.result;
            }
        }

        // If a prompt is defined in the script, use dc.prompt to call it.
        // This prompt must be a valid dialog defined somewhere in your code!
        if (line.prompt) {
            try {
                return await dc.prompt(line.prompt.id, line.text);
            } catch (err) {
                console.error(err);
                await dc.context.sendActivity(`Failed to start prompt ${ line.prompt.id }`);
                return await step.next();
            }
        // If a dialog is defined in the script, use dc.begin to call it.
        // This will cause the specified dialog to run to completion,
        // after which the script will continue.
        } else if (line.dialog) {
            if (line.text) {
                 await dc.context.sendActivity(line.text);
            }
            try {
                return await dc.begin(line.dialog.id);
            } catch (err) {
                console.error(err);
                await dc.context.sendActivity(`Failed to start dialog ${ line.dialog.id }`);
                return await step.next();
            }
        // If there's nothing but text, send it!
        // This could be extended to include cards and other activity attributes. 
        } else {
            await dc.context.sendActivity(line.text);
            return await step.next();
        }
    }

    async runStep(dc, index, reason, result) {
        if (index < this.script.length) {
            // Update the step index
            const state = dc.activeDialog.state;
            state.stepIndex = index;

            // Create step context
            const nextCalled = false;
            const step = {
                index: index,
                options: state.options,
                reason: reason,
                result: result,
                values: state.values,
                next: async (stepResult) => {
                    if (nextCalled) {
                        throw new Error(`ScriptedStepContext.next(): method already called for dialog and step '${this.id}[${index}]'.`);
                    }

                    return await this.dialogResume(dc, DialogReason.nextCalled, stepResult);
                }
            };

            // Execute step
            return await this.onStep(dc, step);
        } else {

            if (this.onComplete) {
                // call the onComplete function if specified
                await this.onComplete(dc, dc.activeDialog.state.values);
            }

            // End of script so just return to parent
            return await dc.end(result);
        }
    }


}

module.exports = ScriptedDialog;