// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ComponentDialog, WaterfallDialog, ChoicePrompt, NumberPrompt } = require('botbuilder-dialogs');
const { createGame, GameTypes } = require('../models/game');
const { formatMessage } = require('../lg');
const { RollAgainDialog } = require('./rollAgainDialog');

const START_DIALOG = 'start';
const ROLL_AGAIN_DIALOG = 'rollAgain';
const SIDES_PROMPT = 'sidesPrompt';
const COUNT_PROMPT = 'countPrompt';

const SIDES_CHOICES = [
    { value: '4', action: { type: 'imBack', title: '4 Sides', value: '4 Sides' }, synonyms: ['four', 'for', '4 sided', '4 sides'] },
    { value: '6', action: { type: 'imBack', title: '6 Sides', value: '6 Sides' }, synonyms: ['six', 'sex', '6 sided', '6 sides'] },
    { value: '8', action: { type: 'imBack', title: '8 Sides', value: '8 Sides' }, synonyms: ['eight', '8 sided', '8 sides'] },
    { value: '10', action: { type: 'imBack', title: '10 Sides', value: '10 Sides' }, synonyms: ['ten', '10 sided', '10 sides'] },
    { value: '12', action: { type: 'imBack', title: '12 Sides', value: '12 Sides' }, synonyms: ['twelve', '12 sided', '12 sides'] },
    { value: '20', action: { type: 'imBack', title: '20 Sides', value: '20 Sides' }, synonyms: ['twenty', '20 sided', '20 sides'] }
];

const SIDES_VALUE = 'sides';
const COUNT_VALUE = 'count';

/**
 * Dialog that will prompt the user to define the settings for a custom game and then make that the
 * current game and initiate a dice roll.
 */
class CreateGameDialog extends ComponentDialog {
    /**
     * Creates a new instance of CreateGameDialog.
     * @param {string} dialogId Unique ID of the dialog within the outer dialog set.
     * @param {StatePropertyAccessor<Game>} currentGameProperty Property for persisting the current Game being played.
     */
    constructor(dialogId, currentGameProperty) {
        super(dialogId);

        this.currentGameProperty = currentGameProperty;

        // Add control flow dialogs
        this.addDialog(new WaterfallDialog(START_DIALOG, [
            this.askSidesStep.bind(this),
            this.askCountStep.bind(this),
            this.rollDiceStep.bind(this)
        ]));
        this.addDialog(new RollAgainDialog(ROLL_AGAIN_DIALOG, currentGameProperty));

        // Add prompts
        this.addDialog(new ChoicePrompt(SIDES_PROMPT));
        this.addDialog(new NumberPrompt(COUNT_PROMPT, this.countPromptValidator.bind(this)));
    }

    /**
     * Prompts the user for the number of sides on each die.
     * @param {WaterfallStepContext} step Contextual information for the current waterfall step.
     */
    async askSidesStep(step) {
        // Prompt user for number sides for each die.
        return step.prompt(SIDES_PROMPT, {
            prompt: formatMessage('en', 'choose_sides'),
            retryPrompt: formatMessage('en', 'choose_sides_reprompt'),
            choices: SIDES_CHOICES
        });
    }

    /**
     * Prompts the number of dice to roll each time.
     * @param {WaterfallStepContext} step Contextual information for the current waterfall step.
     */
    async askCountStep(step) {
        // Remember number of sides
        // - The FoundChoice.value comes back as a string so needs to be converted to a number.
        step.values[SIDES_VALUE] = parseInt(step.result.value);

        // Prompt user for the count of dice to roll.
        const prompt = formatMessage('en', 'choose_count', step.values);
        return await step.prompt(COUNT_PROMPT, prompt);
    }

    /**
     * Creates a new game object, makes it the current game, and performs an initial dice roll.
     * @param {WaterfallStepContext} step Contextual information for the current waterfall step.
     */
    async rollDiceStep(step) {
        // Remember number of dice
        step.values[COUNT_VALUE] = step.result;

        // Start a new custom game
        const game = createGame(GameTypes.custom, step.values[SIDES_VALUE], step.values[COUNT_VALUE]);
        await this.currentGameProperty.set(step.context, game);

        // Roll dice
        return await step.replaceDialog(ROLL_AGAIN_DIALOG);
    }

    /**
     * Validator for the COUNT_PROMPT.
     * @param {PromptValidatorContext} prompt Contextual information for the current prompt being validated.
     */
    async countPromptValidator(prompt) {
        const { succeeded, value } = prompt.recognized;
        if (succeeded && value >= 1 && value <= 100) {
            // Return only integers
            prompt.recognized.value = Math.floor(value);
            return true;
        } else {
            await prompt.context.sendActivity(formatMessage('en', 'choose_count_reprompt'));
            return false;
        }
    }
}

module.exports.CreateGameDialog = CreateGameDialog;
