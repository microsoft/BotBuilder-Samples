// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { MessageFactory, InputHints } = require('botbuilder');
const { ComponentDialog, TextPrompt, WaterfallDialog } = require('botbuilder-dialogs');

const MAIN_WATERFALL_DIALOG = 'mainWaterfallDialog';

class TangentDialog extends ComponentDialog {
    constructor() {
        super(TangentDialog.name);

        this.addDialog(new TextPrompt(TextPrompt.name))
            .addDialog(new WaterfallDialog(WaterfallDialog.name, [
                this.step1.bind(this),
                this.step2.bind(this)
            ]));

        this.initialDialogId = MAIN_WATERFALL_DIALOG;
    }

    async step1(stepContext) {
        const promptMessage = MessageFactory.text('Tangent step 1 of 2', InputHints.ExpectingInput);
        return await stepContext.prompt(TextPrompt.name, {
            prompt: promptMessage
        });
    }

    async step2(stepContext) {
        const promptMessage = MessageFactory.text('Tangent step 2 of 2', InputHints.ExpectingInput);
        return await stepContext.prompt(TextPrompt.name, {
            prompt: promptMessage
        });
    }
}

module.exports.TangentDialog = TangentDialog;
