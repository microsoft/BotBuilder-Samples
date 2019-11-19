"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
const botbuilder_dialogs_1 = require("botbuilder-dialogs");
const botbuilder_1 = require("botbuilder");
/**
 * AdaptiveCardPrompt catches certain common errors that are passed to validator, if present
 * This allows developers to handle these specific errors as they choose
 * These are given in validator context.recognized.value.error
 */
var AdaptiveCardPromptErrors;
(function (AdaptiveCardPromptErrors) {
    /**
     * Error presented if developer specifies AdaptiveCardPromptSettings.promptId,
     *  but user submits adaptive card input on a card where the ID does not match.
     * This error will also be present if developer AdaptiveCardPromptSettings.promptId,
     *  but forgets to add the promptId to every <submit>.data.promptId in your Adaptive Card.
     */
    AdaptiveCardPromptErrors[AdaptiveCardPromptErrors["userInputDoesNotMatchCardId"] = 0] = "userInputDoesNotMatchCardId";
    /**
     * Error presented if developer specifies AdaptiveCardPromptSettings.requiredIds,
     * but user does not submit input for all required input id's on the adaptive card
     */
    AdaptiveCardPromptErrors[AdaptiveCardPromptErrors["missingRequiredIds"] = 1] = "missingRequiredIds";
    /**
     * Error presented if user enters plain text instead of using Adaptive Card's input fields
     */
    AdaptiveCardPromptErrors[AdaptiveCardPromptErrors["userUsedTextInput"] = 2] = "userUsedTextInput";
})(AdaptiveCardPromptErrors = exports.AdaptiveCardPromptErrors || (exports.AdaptiveCardPromptErrors = {}));
/**
 * Waits for Adaptive Card Input to be received.
 *
 * @remarks
 * This prompt is similar to ActivityPrompt but provides features specific to Adaptive Cards:
 *   * Optionally allow specified input fields to be required
 *   * Optionally ensures input is only valid if it comes from the appropriate card (not one shown previous to prompt)
 *   * Provides ability to handle variety of common user errors related to Adaptive Cards
 * DO NOT USE WITH CHANNELS THAT DON'T SUPPORT ADAPTIVE CARDS
 */
class AdaptiveCardPrompt extends botbuilder_dialogs_1.Dialog {
    /**
     * Creates a new AdaptiveCardPrompt instance
     * @param dialogId Unique ID of the dialog within its parent `DialogSet` or `ComponentDialog`.
     * @param settings (optional) Additional options for AdaptiveCardPrompt behavior
     * @param validator (optional) Validator that will be called each time a new activity is received. Validator should handle error messages on failures.
     */
    constructor(dialogId, settings, validator) {
        super(dialogId);
        if (!settings || !settings.card) {
            throw new Error('AdaptiveCardPrompt requires a card in `AdaptiveCardPromptSettings.card`');
        }
        this.validator = validator;
        this.requiredInputIds = settings.requiredInputIds || [];
        this.throwIfNotAdaptiveCard(settings.card);
        this.card = settings.card;
        // Don't allow promptId to be something falsy
        if (settings.promptId !== undefined && !settings.promptId) {
            throw new Error('AdaptiveCardPromptSettings.promptId cannot be a falsy string');
        }
        this.promptId = settings.promptId;
    }
    async beginDialog(dc, options) {
        // Initialize prompt state
        const state = dc.activeDialog.state;
        state.options = options;
        state.state = {};
        // Send initial prompt
        await this.onPrompt(dc.context, state.state, state.options, false);
        return botbuilder_dialogs_1.Dialog.EndOfTurn;
    }
    async onPrompt(context, state, options, isRetry) {
        // Since card is passed in via AdaptiveCardPromptSettings, PromptOptions may not be used.
        // Ensure we're working with RetryPrompt, as applicable
        let prompt;
        if (options) {
            prompt = isRetry && options.retryPrompt ? options.retryPrompt || {} : options.prompt || {};
        }
        else {
            prompt = {};
        }
        // Clone the correct prompt so that we don't affect the one saved in state
        let clonedPrompt = JSON.parse(JSON.stringify(prompt));
        // Create a prompt if user didn't pass it in through PromptOptions or if they passed in a string
        if (!clonedPrompt || typeof (prompt) != 'object' || Object.keys(prompt).length === 0) {
            clonedPrompt = {
                text: typeof (prompt) === 'string' ? prompt : undefined,
            };
        }
        // Depending on how the prompt is called, when compiled to JS, activity attachments may be on prompt or options
        const existingAttachments = clonedPrompt.attachments || options ? options['attachments'] : [];
        // Add Adaptive Card as last attachment (user input should go last), keeping any others
        clonedPrompt.attachments = existingAttachments ? [...existingAttachments, this.card] : [this.card];
        await context.sendActivity(clonedPrompt, undefined, botbuilder_1.InputHints.ExpectingInput);
    }
    // Override continueDialog so that we can catch activity.value (which is ignored, by default)
    async continueDialog(dc) {
        // Perform base recognition
        const state = dc.activeDialog.state;
        const recognized = await this.onRecognize(dc.context);
        if (state.state['attemptCount'] === undefined) {
            state.state['attemptCount'] = 1;
        }
        else {
            state.state['attemptCount']++;
        }
        let isValid = false;
        if (this.validator) {
            isValid = await this.validator({
                context: dc.context,
                recognized: recognized,
                state: state.state,
                options: state.options,
                attemptCount: state.state['attemptCount']
            });
        }
        else if (recognized.succeeded) {
            isValid = true;
        }
        // Return recognized value or re-prompt
        if (isValid) {
            return await dc.endDialog(recognized.value);
        }
        else {
            // Re-prompt
            if (!dc.context.responded) {
                await this.onPrompt(dc.context, state.state, state.options, true);
            }
            return botbuilder_dialogs_1.Dialog.EndOfTurn;
        }
    }
    async onRecognize(context) {
        // Ignore user input that doesn't come from adaptive card
        if (!context.activity.text && context.activity.value) {
            const value = context.activity.value;
            // Validate it comes from the correct card - This is only a worry while the prompt/dialog has not ended
            if (this.promptId && context.activity.value && context.activity.value['promptId'] != this.promptId) {
                return { succeeded: false, value, error: AdaptiveCardPromptErrors.userInputDoesNotMatchCardId };
            }
            // Check for required input data, if specified in AdaptiveCardPromptSettings
            const missingIds = [];
            this.requiredInputIds.forEach((id) => {
                if (!context.activity.value[id] || !context.activity.value[id].trim()) {
                    missingIds.push(id);
                }
            });
            // User did not submit inputs that were required
            if (missingIds.length > 0) {
                return { succeeded: false, value, missingIds, error: AdaptiveCardPromptErrors.missingRequiredIds };
            }
            return { succeeded: true, value };
        }
        else {
            // User used text input instead of card input
            return { succeeded: false, error: AdaptiveCardPromptErrors.userUsedTextInput };
        }
    }
    throwIfNotAdaptiveCard(cardAttachment) {
        const adaptiveCardType = 'application/vnd.microsoft.card.adaptive';
        if (!cardAttachment || !cardAttachment.content) {
            throw new Error('No Adaptive Card provided. Include in the constructor or PromptOptions.prompt.attachments');
        }
        else if (!cardAttachment.contentType || cardAttachment.contentType !== adaptiveCardType) {
            throw new Error(`Attachment is not a valid Adaptive Card.\n` +
                `Ensure card.contentType is '${adaptiveCardType}'\n` +
                `and card.content contains the card json`);
        }
    }
}
exports.AdaptiveCardPrompt = AdaptiveCardPrompt;
