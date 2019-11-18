// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { AdaptiveCardPrompt } = require('../adaptiveCardPrompt');
const { CardFactory } = require('botbuilder');
const {
    ComponentDialog,
    DialogSet,
    DialogTurnStatus,
    WaterfallDialog
} = require('botbuilder-dialogs');

const WATERFALL_DIALOG = 'WATERFALL_DIALOG';
const ADAPTIVE_CARD_PROMPT = 'ADAPTIVE_CARD_PROMPT';

class AdaptiveCardPromptDialog extends ComponentDialog {
    constructor() {
        super('adaptiveCardPromptDialog');

        // Load an adaptive card
        const cardJson = require('../resources/adaptiveCard.json');
        const card = CardFactory.adaptiveCard(cardJson);

        // Configure settings - All optional
        const promptSettings = { card: card };

        this.addDialog(new AdaptiveCardPrompt(ADAPTIVE_CARD_PROMPT, null, promptSettings));

        this.addDialog(new WaterfallDialog(WATERFALL_DIALOG, [
            this.showCard.bind(this),
            this.displayResults.bind(this)
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

    async showCard(stepContext) {
        // Call the prompt
        return await stepContext.prompt(ADAPTIVE_CARD_PROMPT);
    }

    async displayResults(stepContext) {
        // Use the result
        const result = stepContext.result;
        const resultString = Object.keys(result).map((key) => `Key: ${ key }   |   Value: ${ result[key] }`).join('\n');
        await stepContext.context.sendActivity(`Your input:\n\n${ resultString }`);
        return await stepContext.endDialog();
    }
}

module.exports.AdaptiveCardPromptDialog = AdaptiveCardPromptDialog;
