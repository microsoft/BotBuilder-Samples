// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { AdaptiveCardPrompt } = require('../adaptiveCardPrompt');
const { ComponentDialog, WaterfallDialog } = require('botbuilder-dialogs');
const { CardFactory } = require('botbuilder');

const ADAPTIVE_CARD_DIALOG = 'ADAPTIVE_CARD_DIALOG';

class AdaptiveCardDialog extends ComponentDialog {
    constructor() {
        super(ADAPTIVE_CARD_DIALOG);

        // Load an adaptive card
        const cardJson = require('../resources/adaptiveCard.json');
        const card = CardFactory.adaptiveCard(cardJson);

        // Configure settings - All optional
        const promptSettings = { card: card };

        // Initialize the prompt
        const adaptiveCardPrompt = new AdaptiveCardPrompt('adaptiveCardPrompt', null, promptSettings);

        this.addDialog(adaptiveCardPrompt);

        this.addDialog(new WaterfallDialog(ADAPTIVE_CARD_DIALOG, [
            this.showCard.bind(this),
            this.displayResults.bind(this)
        ]));

        this.initialDialogId = ADAPTIVE_CARD_DIALOG;
    }

    async showCard(stepContext) {
        // Call the prompt
        return await stepContext.prompt('adaptiveCardPrompt');
    }

    async displayResults(stepContext) {
        // Use the result
        const result = stepContext.result;
        const resultString = Object.keys(result).map((key) => `Key: ${ key }   |   Value: ${ result[key] }`).join('\n');
        await stepContext.context.sendActivity(`Your input:\n\n${ resultString }`);
        return await stepContext.endDialog();
    }
}

module.exports.AdaptiveCardDialog = AdaptiveCardDialog;
module.exports.ADAPTIVE_CARD_DIALOG = ADAPTIVE_CARD_DIALOG;
