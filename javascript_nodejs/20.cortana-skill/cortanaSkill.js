// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityTypes } = require('botbuilder');
const { MainDialog, DialogTurnStatus } = require('botbuilder-dialogs')

const DIALOG_STATE_PROPERTY = 'dialogState';

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
class CortanaSkill extends MainDialog {
    /**
     * Creates a new instance of the CortanaSkill class.
     * @param {conversationState} conversationState The bots conversation state object. 
     */
    constructor(conversationState) {
        super(conversationState.createProperty(DIALOG_STATE_PROPERTY));

        this.conversationState = conversationState;
    }

    async onBeginDialog(innerDC, options) {
        return await this.onContinueDialog(innerDC);
    }

    async onContinueDialog(innerDC) {
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
            result = await this.onRunTurn(innerDC);
        }

        // Check for end of skill and clear the conversation state if detected.
        if (skillEnded) {
            this.conversationState.clear(innerDC.context);
            result = { status: DialogTurnStatus.cancelled };
        }
        return result;
        
    }

    /**
     * Abstract method that MUST be implemented by the derived skill class. Implementors should
     * add logic for routing incoming requests to the skill.
     * @param {DialogContext} dialogContext Dialog context for the current turn of conversation with the user.
     */
    async onRunTurn(dialogContext) {
        throw new Error(`CortanaSkill.onRunTurn(): not implemented. The derived class MUST implement this method.`);
    }
}
module.exports.CortanaSkill = CortanaSkill;