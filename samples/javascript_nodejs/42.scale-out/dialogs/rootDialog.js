// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { MessageFactory } = require('botbuilder');
const { ComponentDialog, DialogSet, DialogTurnStatus, NumberPrompt, WaterfallDialog } = require('botbuilder-dialogs');

const NUMBER_PROMPT = 'NUMBER_PROMPT';
const WATERFALL_DIALOG = 'WATERFALL_DIALOG';

class RootDialog extends ComponentDialog {
    constructor() {
        super('RootDialog');
        // Define the main dialog and its related components.
        this.addDialog(new NumberPrompt(NUMBER_PROMPT));
        this.addDialog(new WaterfallDialog(WATERFALL_DIALOG, [
            this.firstStep.bind(this),
            this.secondStep.bind(this),
            this.finalStep.bind(this)
        ]));

        this.initialDialogId = WATERFALL_DIALOG;
    }

    /**
     * The run method handles the incoming activity (in the form of a TurnContext) and passes it through the dialog system.
     * If no dialog is active, it will start the default dialog.
     * @param {*} turnContext
     * @param {*} accessor
     */
    async run(turnContext, accessor) {
        const dialogSet = new DialogSet(accessor);
        dialogSet.add(this);

        const dialogContext = await dialogSet.createContext(turnContext);
        const results = await dialogContext.continueDialog();
        if (results.status === DialogTurnStatus.empty) {
            await dialogContext.beginDialog(this.id);
        }
    }

    /**
     * First step in the waterfall dialog. Prompts the user for a command.
     */
    async firstStep(stepContext) {
        // WaterfallStep always finishes with the end of the Waterfall or with another dialog; here it is a Prompt Dialog.
        const promptOptions = { prompt: 'Enter a number.' };

        return await stepContext.prompt(NUMBER_PROMPT, promptOptions);
    }

    /**
     * Second step in the waterfall.
     */
    async secondStep(stepContext) {
        const first = stepContext.result;
        stepContext.values.first = first;
        const promptOptions = { prompt: `I have ${ first } now enter another number` };
        return await stepContext.prompt(NUMBER_PROMPT, promptOptions);
    }

    /**
     * This is the final step in the main waterfall dialog.
     */
    async finalStep(stepContext) {
        const first = stepContext.values.first;
        const second = stepContext.result;
        stepContext.values.second = second;

        const msg = `The result of the first minus the second is ${ first - second }.`;
        await stepContext.context.sendActivity(MessageFactory.text(msg, msg));
        return await stepContext.endDialog();
    }
}

module.exports.RootDialog = RootDialog;
