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
 * The bots main skill class responsible for routing incoming requests to the skill. 
 */
class DiceRollerSkill extends CortanaSkill {
    /**
     * Creates a new instance of DiceRollerSkill.
     * @param {conversationState} conversationState The bots conversation state object. 
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
     * Implements the skills logic for routing incoming requests. 
     * @param {DialogContext} dialogContext Dialog context for the current turn of conversation.  
     */
    async onRunTurn(dialogContext) {
        // Check for interruptions
        const activity = dialogContext.context.activity;
        const isMessage = activity.type === ActivityTypes.Message;
        if (isMessage) {
            const utterance = activity.text || '';
            if (/(roll|role|throw|shoot).*(dice|die|dye|bones)/i.test(utterance) || /new game/i.test(utterance)) {
                // Create a new game
                return await beginRootDialog(dialogContext, CREATE_GAME_DIALOG);

            } else if (/(roll|role|throw|shoot) again/i.test(utterance)) {
                // Check for existing game
                const game = await this.currentGameProperty.get(dialogContext.context);
                if (game) {
                    // Roll again
                    return await beginRootDialog(dialogContext, ROLL_AGAIN_DIALOG);
                } else {
                    // Create a new game
                    return await beginRootDialog(dialogContext, CREATE_GAME_DIALOG);
                }

            } else if (/(play|start).*(craps)/i.test(utterance)) {
                // Start a new game of craps
                const game = createGame(GameTypes.craps);
                await this.currentGameProperty.set(dialogContext.context, game);

                // Initiate a dice roll
                return await beginRootDialog(dialogContext, ROLL_AGAIN_DIALOG);

            } else if (/(help|hello|hi)/i.test(utterance)) {
                // Show help card
                return await beginRootDialog(dialogContext, HELP_DIALOG);

            } else if (/(cancel|close|goodbye)/i.test(utterance)) {
                // Send EndOfConversation to end the current skill invocation
                await dialogContext.context.sendActivity({
                    type: ActivityTypes.EndOfConversation,
                    code: EndOfConversationCodes.UserCancelled
                });
                return await dialogContext.cancelAllDialogs();
            }
        }

        // Continue any current dialog
        let result = await dialogContext.continueDialog();
        
        // Show help card as a fallback
        if (result.status === DialogTurnStatus.empty && isMessage) {
            result = beginRootDialog(dialogContext, HELP_DIALOG);
        }
        return result;
    }
}
module.exports.DiceRollerSkill = DiceRollerSkill;

/**
 * Cancels any active dialog before starting a new dialog. 
 * @param {DialogContext} dialogContext Dialog context for the current turn of conversation. 
 * @param {string} dialogId ID of the dialog to start.
 */
async function beginRootDialog(dialogContext, dialogId) {
    // Cancel any running dialogs
    await dialogContext.cancelAllDialogs();

    // Start new dialog
    return await dialogContext.beginDialog(dialogId);
}
