// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { CardFactory, MessageFactory, InputHints } = require('botbuilder');
const { ComponentDialog, WaterfallDialog } = require('botbuilder-dialogs');
const { formatMessage, getText } = require('../lg');
const ssml = require('../ssml');

const START_DIALOG = 'start';

/**
 * Dialog that implements the skills main dice rolling logic.  This dialog assumes that there is an
 * active Game object assigned to the currentGameProperty and will report an error to the user if
 * a game isn't found.
 */
class RollAgainDialog extends ComponentDialog {
    /**
     * Creates a new instance of RollAgainDialog.
     * @param {string} dialogId
     * @param {StatePropertyAccessor<Game>} currentGameProperty Property for persisting the current Game being played.
     */
    constructor(dialogId, currentGameProperty) {
        super(dialogId);

        this.currentGameProperty = currentGameProperty;
        // Add control flow dialogs
        this.addDialog(new WaterfallDialog(START_DIALOG, [
            this.ensureGameStep.bind(this),
            this.rollDiceStep.bind(this)
        ]));
    }

    /**
     * Ensures that there is an active game configured and passes the game to the rollDiceStep().
     * @param {WaterfallStepContext} step Contextual information for the current waterfall step.
     */
    async ensureGameStep(step) {
        const game = await this.currentGameProperty.get(step.context);
        if (game) {
            return await step.next(game);
        } else {
            await step.context.sendActivity(formatMessage('en', 'roll_again_missing_game'));
            return await step.endDialog();
        }
    }

    /**
     * Implements the actual logic to roll a set of dice and send the result to the user as a card.
     * @param {WaterfallStepContext} step Contextual information for the current waterfall step.
     */
    async rollDiceStep(step) {
        const game = step.result;

        // Generate rolls
        let total = 0;
        const rolls = [];
        for (let i = 0; i < game.count; i++) {
            let roll = Math.floor(Math.random() * game.sides) + 1;
            if (roll > game.sides) {
                // Accounts for 1 in a million chance random() generated a 1.0
                roll = game.sides;
            }
            total += roll;
            rolls.push(roll);
        }

        // Format roll results
        let results = '';
        const multiLine = rolls.length > 5;
        for (let i = 0; i < rolls.length; i++) {
            if (i > 0) {
                results += ' . ';
            }
            results += rolls[i];
        }

        // Render results using a Hero Card
        const title = multiLine ? getText('en', 'card_title') : results;
        const card = CardFactory.heroCard(title, undefined, ['Roll Again', 'New Game']);
        card.subtitle = getText('en', game.count > 1 ? 'card_subtitle_plural' : 'card_subtitle_singular', game);
        if (multiLine) {
            card.text = results;
        }
        const msg = MessageFactory.attachment(card);

        // Determine bots reaction for speech purposes
        var reaction = 'normal';
        var max = game.count * game.sides;
        var score = total / max;
        if (score === 1.0) {
            reaction = 'best';
        } else if (score === 0) {
            reaction = 'worst';
        } else if (score <= 0.3) {
            reaction = 'bad';
        } else if (score >= 0.8) {
            reaction = 'good';
        }
        // Check for special craps rolls
        if (game.type === 'craps') {
            switch (total) {
            case 2:
            case 3:
            case 12:
                reaction = 'craps_lose';
                break;
            case 7:
                reaction = 'craps_seven';
                break;
            case 11:
                reaction = 'craps_eleven';
                break;
            default:
                reaction = 'craps_retry';
                break;
            }
        }

        // Build up spoken response
        var spoken = '';
        if (game.turn === 0) {
            spoken += getText('en', 'start_' + game.type + '_game_ssml') + ' ';
        }
        spoken += getText('en', reaction + '_roll_reaction_ssml');
        msg.speak = ssml.speak(spoken);

        // Increment number of turns and store game to roll again
        game.turn++;
        await this.currentGameProperty.set(step.context, game);

        // Send card and bots reaction to user.
        msg.inputHint = InputHints.AcceptingInput;
        await step.context.sendActivity(msg);
        return await step.endDialog();
    }
}

module.exports.RollAgainDialog = RollAgainDialog;
