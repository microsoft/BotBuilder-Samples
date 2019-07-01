// Note: This is an edit of the compiled version of: 
// https://github.com/mdrichardson/botbuilder-js/blob/adaptiveCardPrompt/libraries/botbuilder-dialogs/src/prompts/adaptiveCardPrompt.ts

const { Dialog } = require("botbuilder-dialogs");
const { InputHints } = require("botbuilder");
/**
 * Waits for Adaptive Card Input to be received.
 *
 * @remarks
 * This prompt is similar to ActivityPrompt but provides features specific to Adaptive Cards:
 *   * Card can be passed in constructor or as prompt/reprompt activity attachment
 *   * Includes validation for specified required input fields
 *   * Displays custom message if user replies via text and not card input
 *   * Ensures input is only valid if it comes from the appropriate card (not one shown previous to prompt)
 * DO NOT USE WITH CHANNELS THAT DON'T SUPPORT ADAPTIVE CARDS
 */
class AdaptiveCardPrompt extends Dialog {
    /**
     * Creates a new AdaptiveCardPrompt instance
     * @param dialogId Unique ID of the dialog within its parent `DialogSet` or `ComponentDialog`.
     * @param validator (optional) Validator that will be called each time a new activity is received. Validator should handle error messages on failures.
     * @param settings (optional) Additional options for AdaptiveCardPrompt behavior
     */
    constructor(dialogId, validator, settings) {
        super(dialogId);
        this.usesCustomPromptId = false;
        // Necessary for when this compiles to js since strictPropertyInitialization is false/unset in tsconfig
        settings = typeof (settings) === 'object' ? settings : {};
        this.validator = validator;
        this._inputFailMessage = settings.inputFailMessage || 'Please fill out the Adaptive Card';
        this._requiredInputIds = settings.requiredInputIds || [];
        this._missingRequiredInputsMessage = settings.missingRequiredInputsMessage || 'The following inputs are required';
        this._attemptsBeforeCardRedisplayed = settings.attemptsBeforeCardRedisplayed || 3;
        this._card = settings.card;
        if (settings.promptId) {
            this._promptId = settings.promptId;
            this.usesCustomPromptId = true;
        }
    }
    get inputFailMessage() {
        return this._inputFailMessage;
    }
    set inputFailMessage(message) {
        this._inputFailMessage = message;
    }
    get requiredInputIds() {
        return this._requiredInputIds;
    }
    set requiredInputIds(ids) {
        this._requiredInputIds = ids;
    }
    get missingRequiredInputsMessage() {
        return this._missingRequiredInputsMessage;
    }
    set missingRequiredInputsMessage(message) {
        this._missingRequiredInputsMessage = message;
    }
    get attemptsBeforeCardRedisplayed() {
        return this._attemptsBeforeCardRedisplayed;
    }
    set attemptsBeforeCardRedisplayed(attempts) {
        this._attemptsBeforeCardRedisplayed = attempts;
    }
    get promptId() {
        return this._promptId;
    }
    set promptId(id) {
        this.usesCustomPromptId = true;
        this._promptId = id;
    }
    get card() {
        return this._card;
    }
    set card(card) {
        this._card = card;
    }
    async beginDialog(dc, options) {
        // Initialize prompt state
        const state = dc.activeDialog.state;
        state.options = options;
        state.state = {};
        // Send initial prompt
        await this.onPrompt(dc.context, state.state, state.options, false);
        return Dialog.EndOfTurn;
    }
    async onPrompt(context, state, options, isRetry) {
        // Should use GUID for C# -- it isn't native to Node, so this keeps dependencies down
        // Only the most recently-prompted card submission is accepted
        this._promptId = this.usesCustomPromptId ? this._promptId : `${Math.random()}`;
        let prompt = isRetry && options.retryPrompt ? options.retryPrompt : options.prompt;
        // Create a prompt if user didn't pass it in through PromptOptions
        if (!prompt || Object.keys(prompt).length === 0 || typeof (prompt) != 'object' || !prompt.attachments || prompt.attachments.length === 0) {
            prompt = {
                attachments: [],
                text: typeof (prompt) === 'string' ? prompt : undefined,
            };
        }
        // Use card passed in PromptOptions or if it doesn't exist, use the one passed in from the constructor
        const card = prompt.attachments && prompt.attachments[0] ? prompt.attachments[0] : this._card;
        this.validateIsCard(card, isRetry);
        prompt.attachments = [this.addPromptIdToCard(card)];
        await context.sendActivity(prompt, undefined, InputHints.ExpectingInput);
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
        if (this.validator && recognized.succeeded) {
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
            // Re-prompt, conditionally display card again
            if (state.state['attemptCount'] % this._attemptsBeforeCardRedisplayed === 0) {
                await this.onPrompt(dc.context, state.state, state.options, true);
            }
            return await Dialog.EndOfTurn;
        }
    }
    async onRecognize(context) {
        // Ignore user input that doesn't come from adaptive card
        if (!context.activity.text && context.activity.value) {
            // Validate it comes from the correct card - This is only a worry while the prompt/dialog has not ended
            if (context.activity.value && context.activity.value['promptId'] != this._promptId) {
                return { succeeded: false };
            }
            // Check for required input data, if specified in AdaptiveCardPromptSettings
            let missingIds = [];
            this._requiredInputIds.forEach((id) => {
                if (!context.activity.value[id] || !context.activity.value[id].trim()) {
                    missingIds.push(id);
                }
            });
            // Alert user to missing data
            if (missingIds.length > 0) {
                if (this._missingRequiredInputsMessage) {
                    await context.sendActivity(`${this._missingRequiredInputsMessage}: ${missingIds.join(', ')}`);
                }
                return { succeeded: false };
            }
            return { succeeded: true, value: context.activity.value };
        }
        else {
            // User used text input instead of card input or is missing required Inputs
            if (this._inputFailMessage) {
                await context.sendActivity(this._inputFailMessage);
            }
            return { succeeded: false };
        }
    }
    validateIsCard(card, isRetry) {
        const adaptiveCardType = 'application/vnd.microsoft.card.adaptive';
        const cardLocation = isRetry ? 'retryPrompt' : 'prompt';
        if (!card || !card.content) {
            throw new Error(`No Adaptive Card provided. Include in the constructor or PromptOptions.${cardLocation}.attachments[0]`);
        }
        else if (!card.contentType || card.contentType !== adaptiveCardType) {
            throw new Error(`Attachment is not a valid Adaptive Card.\n` +
                `Ensure card.contentType is '${adaptiveCardType}'\n` +
                `and card.content contains the card json`);
        }
    }
    addPromptIdToCard(card) {
        card.content = this.deepSearchJsonForActionsAndAddPromptId(card.content);
        return card;
    }
    deepSearchJsonForActionsAndAddPromptId(json) {
        const submitAction = 'Action.Submit';
        const showCardAction = 'Action.ShowCard';
        for (const key in json) {
            // Search for all submits in actions
            if (key === 'actions') {
                for (const action in json[key]) {
                    if (json[key][action].type && json[key][action].type === submitAction) {
                        json[key][action].data = { ...json[key][action].data, ...{ promptId: this._promptId } };
                        // Recursively search Action.ShowCard for Submits within the nested card
                    }
                    else if (json[key][action].type && json[key][action].type === showCardAction) {
                        json[key][action] = this.deepSearchJsonForActionsAndAddPromptId(json[key][action]);
                    }
                }
                // Search for all submits in selectActions
            }
            else if (key === 'selectAction') {
                if (json[key].type && json[key].type === submitAction) {
                    json[key].data = { ...json[key].data, ...{ promptId: this._promptId } };
                    // Recursively search Action.ShowCard for Submits within the nested card
                }
                else if (json[key].type && json[key].type === showCardAction) {
                    json[key] = this.deepSearchJsonForActionsAndAddPromptId(json[key]);
                }
                // Recursively search all other objects
            }
            else if (json[key] && typeof json[key] === 'object') {
                json[key] = this.deepSearchJsonForActionsAndAddPromptId(json[key]);
            }
        }
        return json;
    }
}
exports.AdaptiveCardPrompt = AdaptiveCardPrompt;
//# sourceMappingURL=adaptiveCardPrompt.js.map