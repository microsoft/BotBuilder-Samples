// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityTypes } = require('botbuilder');
const { ComponentDialog, DialogTurnStatus, DialogSet } = require('botbuilder-dialogs');

const DIALOG_STATE_PROPERTY = 'dialogState';
const DEFAULT_DIALOG_ID = 'skill';

/**
 * Abstract base class for the bots main skill class. Derived classes MUST implement an onRunTurn()
 * method and then add their skills dialogs using this.addDialog().
 *
 * This base class adds logic to clear the bots conversation state anytime an EndOfConversation
 * activity is sent or received.  Cortana will send an EndOfConversation activity should the client
 * end the current skill (user closes window) and the bot/skill can send an EndOfConversation
 * activity anytime they wish to end the current skill.
 *
 * In some cases Cortana will re-use the same conversationId for the next invocation of the skill so
 * as a best practice you should clear your bots conversation state anytime an EndOfConversation
 * activity is detected.
 */
class CortanaSkill extends ComponentDialog {
    /**
     * Creates a new instance of the CortanaSkill class.
     * @param {conversationState} conversationState The bots conversation state object.
     */
    constructor(conversationState, dialogId) {
        super(dialogId || DEFAULT_DIALOG_ID);

        this.conversationState = conversationState;

        // The skill is our bots root dialog so we need to create a dialog set and add ourselves
        // to it. This dialog set will *only* contain the root dialog and is need to preserve our
        // overall routing model.
        const dialogState = conversationState.createProperty(DIALOG_STATE_PROPERTY);
        this.mainDialogSet = new DialogSet(dialogState);
        this.mainDialogSet.add(this);
    }

    /** Override base onBeginDialog() to call listenForEndOfConversation(). */
    onBeginDialog(innerDC, options) {
        return this.listenForEndOfConversation(innerDC, options);
    }

    /** Override base onContinueDialog() to call listenForEndOfConversation(). */
    onContinueDialog(innerDC) {
        return this.listenForEndOfConversation(innerDC);
    }

    /** Wrap onRunTurn() with logic to handle EndOfConversation events. */
    async listenForEndOfConversation(innerDC, options) {
        // Process incoming or outgoing EndOfConversation events.
        let skillEnded = false;
        let result;
        if (innerDC.context.activity.type === ActivityTypes.EndOfConversation) {
            // Skill ended (user closed the window)
            skillEnded = true;
        } else {
            // Listen for bot to send EndOfConversation
            innerDC.context.onSendActivities(async (ctx, activities, next) => {
                activities.forEach(a => {
                    if (a.type === ActivityTypes.EndOfConversation) {
                        skillEnded = true;
                    }
                });
                return await next();
            });

            // Run turn
            result = await this.onRunTurn(innerDC, options);
        }

        // Check for end of skill and clear the conversation state if detected.
        if (skillEnded) {
            this.conversationState.clear(innerDC.context);
            result = { status: DialogTurnStatus.cancelled };
        }
        return result;
    }

    /**
     * Routes the incoming activity to the skills dialogs. For complex skills with multiple dialogs
     * and that support features like interruption you'll want to override this method and provide
     * your own conversation routing logic.
     * @param {DialogContext} innerDC Dialog context for the current turn of conversation with the user.
     * @param {object} options (Optional) options passed in for the first turn with the skill.
     */
    async onRunTurn(innerDC, options) {
        // Attempt to continue the current dialog
        let result = await innerDC.continueDialog();

        // Start initial dialog if not running
        if (result.status === DialogTurnStatus.empty && innerDC.context.activity.type === ActivityTypes.Message) {
            result = await innerDC.beginDialog(this.initialDialogId, options);
        }
        return result;
    }

    /**
     * Called from within the bots BotAdapter.processActivity() callback to route a
     * received activity to the appropriate dialog.
     * @param {TurnContext} context Context for the current turn of conversation with the user.
     * @param {object} options (Optional) options that can be used to configure the skill on the first turn of conversation with the user.
     */
    async run(context, options) {
        if (!context) {
            throw new Error('CortanaSkill.run(): context is undefined or null');
        }

        // Create a dialog context and try to continue running the current dialog
        const dc = await this.mainDialogSet.createContext(context);
        let result = await dc.continueDialog();

        // Start the main dialog if there wasn't a running one
        if (result.status === DialogTurnStatus.empty) {
            result = await dc.beginDialog(this.id, options);
        }
        return result;
    }
}

module.exports.CortanaSkill = CortanaSkill;
