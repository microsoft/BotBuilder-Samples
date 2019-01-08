// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { MessageFactory } = require('botbuilder');
const { ComponentDialog, NumberPrompt, WaterfallDialog } = require('botbuilder-dialogs');

class RootDialog extends ComponentDialog {

    constructor() {
        super('root');
        this.addDialog(RootDialog.createWaterfall());
        this.addDialog(new NumberPrompt('number'));
    }

    static createWaterfall() {
        return new WaterfallDialog("root-waterfall", [ RootDialog.step1, RootDialog.step2, RootDialog.step3 ]);
    }

    static async step1(stepContext) {
        return await stepContext.prompt('number', { prompt: MessageFactory.text('Enter a number.') });
    }

    static async step2(stepContext) {
        const first = stepContext.result;
        stepContext.values.first = first;
        return await stepContext.prompt('number', { prompt: MessageFactory.text(`I have ${first} now enter another number`) });
    }

    static async step3(stepContext) {
        const first = stepContext.values.first;
        const second = stepContext.result;
        await stepContext.Context.sendActivity(MessageFactory.text(`The result of the first minus the second is ${first - second}.`));
        return await stepContext.EndDialog();
    }
}

module.exports.RootDialog = RootDialog;
