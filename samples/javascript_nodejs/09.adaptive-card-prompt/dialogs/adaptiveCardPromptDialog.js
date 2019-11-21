// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { AdaptiveCardPrompt } = require('../adaptiveCardPrompt');
const { CardFactory } = require('botbuilder');
const {
    ChoiceFactory,
    ChoicePrompt,
    ComponentDialog,
    DialogSet,
    DialogTurnStatus,
    WaterfallDialog
} = require('botbuilder-dialogs');

const WATERFALL_DIALOG = 'WATERFALL_DIALOG';
const CHOICE_PROMPT = 'CHOICE';
const SIMPLE_ADAPTIVE_CARD_PROMPT = 'SIMPLE';
const COMPLEX_ADAPTIVE_CARD_PROMPT = 'COMPLEX';

class AdaptiveCardPromptDialog extends ComponentDialog {
    constructor() {
        super('adaptiveCardPromptDialog');

        // Load adaptive cards
        const simpleAdaptiveCard = this.createCard('../resources/simpleAdaptiveCard.json');
        const simpleSettings = {
            card: simpleAdaptiveCard
        };
        this.addDialog(new AdaptiveCardPrompt(SIMPLE_ADAPTIVE_CARD_PROMPT, simpleSettings));

        const complexAdaptiveCard = this.createCard('../resources/complexAdaptiveCard.json');
        const complexSettings = {
            card: complexAdaptiveCard,
            requiredInputIds: [
                'textInput'
            ],
            promptId: 'complexCard'
        };
        this.addDialog(new AdaptiveCardPrompt(COMPLEX_ADAPTIVE_CARD_PROMPT, complexSettings, this.validateInput));

        this.addDialog(new ChoicePrompt(CHOICE_PROMPT));

        this.addDialog(new WaterfallDialog(WATERFALL_DIALOG, [
            this.whichCard.bind(this),
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

    async whichCard(stepContext) {
        const text = 'Please choose which card to display:\n\n' +
            '* **Simple**: Displays basic actions of AdaptiveCardPrompt\n' +
            '* **Complex**: Display a complex adaptive card that catches common user errors with a validator';
        return await stepContext.prompt(CHOICE_PROMPT, {
            prompt: text,
            choices: ChoiceFactory.toChoices(['Simple', 'Complex'])
        });
    }

    async showCard(stepContext) {
        const wrongCard = this.createCard('../resources/wrongAdaptiveCard.json');
        switch (stepContext.result.value) {
        case 'Simple':
            return await stepContext.prompt(SIMPLE_ADAPTIVE_CARD_PROMPT);
        case 'Complex':
            // Attach an additional card to the prompt so the user can try to reproduce the "wrong card id" error
            return await stepContext.prompt(COMPLEX_ADAPTIVE_CARD_PROMPT, { attachments: [wrongCard] });
        default:
            break;
        }
    }

    async displayResults(stepContext) {
        await stepContext.context.sendActivity('Success!');

        // Send the result to the user
        const result = stepContext.result.data;
        const resultString = Object.keys(result).map((key) => `* **${ key }**: ${ result[key] }`).join('\n\n');
        await stepContext.context.sendActivity(`Your input:\n\n${ resultString }`);

        await stepContext.context.sendActivity('Type anything to start again.');

        return await stepContext.endDialog();
    }

    createCard(filePath) {
        const cardJson = require(filePath);
        return CardFactory.adaptiveCard(cardJson);
    }

    async validateInput(promptContext) {
        // Check for known user errors - This is based on the AdaptiveCardPromptErrors enum.
        const missingIds = promptContext.recognized.value.missingIds;
        switch (promptContext.recognized.value.error) {
        // No errors
        case 0:
            break;
        // User entered input in a card with an ID that does not match the one the prompt sent (AdaptiveCardPromptSettings.promptId)
        case 1:
            await promptContext.context.sendActivity('Please submit input on the correct card');
            return false;
        // User did not enter input for input IDs that are required (AdaptiveCardPromptSettings.requiredIds)
        case 2:
            await promptContext.context.sendActivity(
                'Please fill out all of the required inputs\n\n' +
                'Required inputs:\n\n' +
                `${ missingIds.join('\n\n') }`
            );
            return false;
        // User entered text input instead of using the Adaptive Card
        case 3:
            await promptContext.context.sendActivity('Please use the card to submit input');
            return false;
        default:
            break;
        }

        return true;
    }
}

module.exports.AdaptiveCardPromptDialog = AdaptiveCardPromptDialog;
