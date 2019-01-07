// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityTypes, EndOfConversationCodes } = require('botbuilder');
const { DialogTurnStatus } = require('botbuilder-dialogs');
const { CortanaSkill } = require('../cortanaSkill');
const { CreateGameDialog } = require('./createGameDialog');
const { RollAgainDialog } = require('./rollAgainDialog');
const { HelpDialog } = require('./helpDialog');
const { createGame, GameTypes } = require('../models/game');

const CURRENT_GAME_PROPERTY = 'currentGame';

const CREATE_GAME_DIALOG = 'createGame';
const ROLL_AGAIN_DIALOG = 'rollAgain';
const HELP_DIALOG = 'help';

/**
<<<<<<< HEAD
 * The bots main skill class responsible for routing incoming requests to the skill. 
=======
 * The bots main skill class responsible for routing incoming requests to the skill.
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
 */
class DiceRollerSkill extends CortanaSkill {
    /**
     * Creates a new instance of DiceRollerSkill.
<<<<<<< HEAD
     * @param {conversationState} conversationState The bots conversation state object. 
=======
     * @param {conversationState} conversationState The bots conversation state object.
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
     */
    constructor(conversationState) {
        super(conversationState);

        this.currentGameProperty = conversationState.createProperty(CURRENT_GAME_PROPERTY);

        // Add root dialogs
        this.addDialog(new CreateGameDialog(CREATE_GAME_DIALOG, this.currentGameProperty));
        this.addDialog(new RollAgainDialog(ROLL_AGAIN_DIALOG, this.currentGameProperty));
        this.addDialog(new HelpDialog(HELP_DIALOG));
    }

    /**
<<<<<<< HEAD
     * Implements the skills logic for routing incoming requests. 
=======
     * Implements the skills logic for routing incoming requests.
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
     * @param {DialogContext} innerDC Dialog context for the current turn of conversation.
     * @param {object} options (Optional) options passed in for the first turn with the skill.
     */
    async onRunTurn(innerDC, options) {
        // Check for interruptions
        const activity = innerDC.context.activity;
        const isMessage = activity.type === ActivityTypes.Message;
        if (isMessage) {
            const utterance = activity.text || '';
            if (/(roll|role|throw|shoot).*(dice|die|dye|bones)/i.test(utterance) || /new game/i.test(utterance)) {
                // Create a new game
                return await beginRootDialog(innerDC, CREATE_GAME_DIALOG);
<<<<<<< HEAD

=======
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
            } else if (/(roll|role|throw|shoot) again/i.test(utterance)) {
                // Check for existing game
                const game = await this.currentGameProperty.get(innerDC.context);
                if (game) {
                    // Roll again
                    return await beginRootDialog(innerDC, ROLL_AGAIN_DIALOG);
                } else {
                    // Create a new game
                    return await beginRootDialog(innerDC, CREATE_GAME_DIALOG);
                }
<<<<<<< HEAD

=======
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
            } else if (/(play|start).*(craps)/i.test(utterance)) {
                // Start a new game of craps
                const game = createGame(GameTypes.craps);
                await this.currentGameProperty.set(innerDC.context, game);

                // Initiate a dice roll
                return await beginRootDialog(innerDC, ROLL_AGAIN_DIALOG);
<<<<<<< HEAD

            } else if (/(help|hello|hi)/i.test(utterance)) {
                // Show help card
                return await beginRootDialog(innerDC, HELP_DIALOG);

=======
            } else if (/(help|hello|hi)/i.test(utterance)) {
                // Show help card
                return await beginRootDialog(innerDC, HELP_DIALOG);
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
            } else if (/(cancel|close|goodbye)/i.test(utterance)) {
                // Send EndOfConversation to end the current skill invocation
                await innerDC.context.sendActivity({
                    type: ActivityTypes.EndOfConversation,
                    code: EndOfConversationCodes.UserCancelled
                });
                return await innerDC.cancelAllDialogs();
            }
        }

        // Continue any current dialog
        let result = await innerDC.continueDialog();
<<<<<<< HEAD
        
=======
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
        // Show help card as a fallback
        if (result.status === DialogTurnStatus.empty && isMessage) {
            result = beginRootDialog(innerDC, HELP_DIALOG);
        }
        return result;
    }
}
module.exports.DiceRollerSkill = DiceRollerSkill;

/**
<<<<<<< HEAD
 * Cancels any active dialog before starting a new dialog. 
 * @param {DialogContext} dialogContext Dialog context for the current turn of conversation. 
=======
 * Cancels any active dialog before starting a new dialog.
 * @param {DialogContext} dialogContext Dialog context for the current turn of conversation.
>>>>>>> 9a1346f23e7379b539e9319c6886e3013dc05145
 * @param {string} dialogId ID of the dialog to start.
 */
async function beginRootDialog(dialogContext, dialogId) {
    // Cancel any running dialogs
    await dialogContext.cancelAllDialogs();

    // Start new dialog
    return await dialogContext.beginDialog(dialogId);
}
