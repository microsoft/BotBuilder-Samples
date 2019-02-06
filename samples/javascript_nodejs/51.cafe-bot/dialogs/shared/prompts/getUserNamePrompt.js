// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
const { TextPrompt } = require('botbuilder-dialogs');
const { MessageFactory, CardFactory } = require('botbuilder');
const { LuisRecognizer } = require('botbuilder-ai');
const { UserProfile } = require('../stateProperties');
const { GetUserNameCard } = require('../../whoAreYou/getUserNameCard');
const InterruptionDispatcher = 'interruptionDispatcherDialog';
// LUIS service type entry for the get user profile LUIS model in the .bot file.
const LUIS_CONFIGURATION = 'getUserProfile';
// LUIS intent names from ./resources/getUserProfile.lu
const WHY_DO_YOU_ASK_INTENT = 'Why_do_you_ask';
const GET_USER_NAME_INTENT = 'Get_user_name';
const NO_NAME_INTENT = 'No_Name';
const NONE_INTENT = 'None';
const CANCEL_INTENT = 'Cancel';
// User name entity from ./resources/getUserProfile.lu
const USER_NAME = 'userName';
const USER_NAME_PATTERN_ANY = 'userName_patternAny';
const TURN_COUNTER_PROPERTY = 'turnCounterProperty';
const HAVE_USER_PROFILE = true;
const NO_USER_PROFILE = false;
const CONFIRM_CANCEL_PROMPT = 'confirmCancelPrompt';
module.exports = class GetUserNamePrompt extends TextPrompt {
    /**
     * Constructor.
     * This is a custom TextPrompt that uses a LUIS model to handle turn.n conversations including interruptions.
     * @param {String} dialogId Dialog ID
     * @param {Object} botConfig Bot configuration
     * @param {Object} userProfileAccessor accessor for user profile property
     * @param {Object} conversationState conversations state
     */
    constructor(dialogId, botConfig, userProfileAccessor, conversationState, onTurnAccessor) {
        if (!dialogId) throw new Error('Missing parameter. Dialog ID is required.');
        if (!botConfig) throw new Error('Missing parameter. Bot configuration is required.');
        if (!userProfileAccessor) throw new Error('Missing parameter. User profile property accessor is required.');
        if (!conversationState) throw new Error('Missing parameter. Conversation state is required.');

        // Call super and provide a prompt validator
        super(dialogId, async (validatorContext) => {
            // Get user profile through the accessor.
            const userProfile = await this.userProfileAccessor.get(validatorContext.context);
            // Get turn counter
            let turnCounter = await this.turnCounterAccessor.get(validatorContext.context);
            turnCounter = (turnCounter === undefined) ? 0 : ++turnCounter;
            // set updated turn counter
            await this.turnCounterAccessor.set(validatorContext.context, turnCounter);
            // Prompt validator
            // Examine if we have a user name and validate it.
            if (userProfile !== undefined && userProfile.userName !== undefined) {
                // We can only accept user names that up to two words.
                if (userProfile.userName.split(' ').length > 2) {
                    await validatorContext.context.sendActivity(`Sorry, I can only accept two words for a name.`);
                    await validatorContext.context.sendActivity(`You can always say 'My name is <your name>' to introduce yourself to me.`);
                    await this.userProfileAccessor.set(validatorContext.context, new UserProfile('Human'));
                    // set updated turn counter
                    await this.turnCounterAccessor.set(validatorContext.context, 0);
                    return HAVE_USER_PROFILE;
                } else {
                    // capitalize user name
                    userProfile.userName = userProfile.userName.charAt(0).toUpperCase() + userProfile.userName.slice(1);
                    // Create user profile and set it to state.
                    await this.userProfileAccessor.set(validatorContext.context, userProfile);
                    return HAVE_USER_PROFILE;
                }
            } else {
                // Evaluate course of action based on the turn counter.
                if (turnCounter >= 1) {
                    // We we need to get user's name right. Include a card.
                    await validatorContext.context.sendActivity({ attachments: [CardFactory.adaptiveCard(GetUserNameCard)] });
                    return NO_USER_PROFILE;
                } else if (turnCounter >= 3) {
                    // We are not going to spend more than 3 turns to get user's name.
                    await validatorContext.context.sendActivity(`Sorry, unfortunately I'm not getting it :(`);
                    await validatorContext.context.sendActivity(`You can always say 'My name is <your name>' to introduce yourself to me.`);
                    await this.userProfileAccessor.set(validatorContext.context, new UserProfile('Human'));
                    // set updated turn counter
                    await this.turnCounterAccessor.set(validatorContext.context, 0);
                    return HAVE_USER_PROFILE;
                }
            }
            return NO_USER_PROFILE;
        });

        // Keep a copy of accessors.
        this.userProfileAccessor = userProfileAccessor;
        this.turnCounterAccessor = conversationState.createProperty(TURN_COUNTER_PROPERTY);
        this.onTurnAccessor = onTurnAccessor;

        // add recognizer
        const luisConfig = botConfig.findServiceByNameOrId(LUIS_CONFIGURATION);
        if (!luisConfig || !luisConfig.appId) throw new Error(`Get User Profile LUIS configuration not found in .bot file. Please ensure you have all required LUIS models created and available in the .bot file. See readme.md for additional information\n`);
        this.luisRecognizer = new LuisRecognizer({
            applicationId: luisConfig.appId,
            endpoint: luisConfig.getEndpoint(),
            // CAUTION: Authoring key is used in this example as it is appropriate for prototyping.
            // When implimenting for deployment/production, assign and use a subscription key instead of an authoring key.
            endpointKey: luisConfig.authoringKey
        });
    }
    /**
     * Override continueDialog.
     *   The override enables
     *     Interruption to be kicked off from right within this dialog.
     *     Ability to leverage a dedicated LUIS model to provide flexible entity filling,
     *     corrections and contextual help.
     *
     * @param {Object} dialog context
     */
    async continueDialog(dc) {
        let context = dc.context;

        // See if we have card input. This would come in through onTurnProperty
        const onTurnProperty = await this.onTurnAccessor.get(context);
        if (onTurnProperty !== undefined) {
            if (onTurnProperty.entities.length !== 0) {
                // find user name in on turn property
                const userNameInOnTurnProperty = onTurnProperty.entities.find(item => item.entityName === USER_NAME);
                if (userNameInOnTurnProperty !== undefined) {
                    await this.updateUserProfileProperty(userNameInOnTurnProperty.entityValue, context);
                    return await super.continueDialog(dc);
                }
            }
        }

        // call LUIS and get results
        const LUISResults = await this.luisRecognizer.recognize(context);
        let topIntent = LuisRecognizer.topIntent(LUISResults);
        if (Object.keys(LUISResults.intents).length === 0) {
            // go with intent in onTurnProperty
            topIntent = (onTurnProperty.intent || 'None');
        }
        // Did user ask for help or said they are not going to give us the name?
        switch (topIntent) {
        case NO_NAME_INTENT: {
            // set user name in profile to Human
            await this.userProfileAccessor.set(context, new UserProfile('Human'));
            return await this.endGetUserNamePrompt(dc);
        }
        case GET_USER_NAME_INTENT: {
            // Find the user's name from LUIS entities list.
            if (USER_NAME in LUISResults.entities) {
                await this.updateUserProfileProperty(LUISResults.entities[USER_NAME][0], context);
                return await super.continueDialog(dc);
            } else if (USER_NAME_PATTERN_ANY in LUISResults.entities) {
                await this.updateUserProfileProperty(LUISResults.entities[USER_NAME_PATTERN_ANY][0], context);
                return await super.continueDialog(dc);
            } else {
                await context.sendActivity(`Sorry, I didn't get that. What's your name?`);
                return await super.continueDialog(dc);
            }
        }
        case WHY_DO_YOU_ASK_INTENT: {
            await context.sendActivity(`I need your name to be able to address you correctly!`);
            await context.sendActivity(MessageFactory.suggestedActions([`I won't give you my name`], `What is your name?`));
            return await super.continueDialog(dc);
        }
        case NONE_INTENT: {
            await this.updateUserProfileProperty(context.activity.text, context);
            return await super.continueDialog(dc);
        } case CANCEL_INTENT: {
            // start confirmation prompt
            return await dc.prompt(CONFIRM_CANCEL_PROMPT, `Are you sure you want to cancel?`);
        }
        default: {
            // Handle interruption.
            const onTurnProperty = await this.onTurnAccessor.get(dc.context);
            return await dc.beginDialog(InterruptionDispatcher, onTurnProperty);
        }
        }
    }
    /**
     * Helper method to end this prompt
     *
     * @param {DialogContext} dc
     */
    async endGetUserNamePrompt(dc) {
        let context = dc.context;
        await context.sendActivity(`No worries. Hello Human, nice to meet you!`);
        await context.sendActivity(`You can always say 'My name is <your name>' to introduce yourself to me.`);
        // End this dialog since user does not wish to proceed further.
        return await dc.endDialog(NO_USER_PROFILE);
    }
    /**
     * Override resumeDialog.
     * This is used to handle user's response to confirm cancel prompt.
     *
     * @param {DialogContext} dc
     * @param {DialogReason} reason
     * @param {Object} result
     */
    async resumeDialog(dc, reason, result) {
        if (result) {
            // User said yes to cancel prompt.
            await dc.context.sendActivity(`Sure. I've cancelled that!`);
            return await dc.cancelAllDialogs();
        } else {
            // User said no to cancel.
            return await super.resumeDialog(dc, reason, result);
        }
    }
    /**
     * Helper method to set user name
     *
     * @param {String} userName
     * @param {Object} context
     */
    async updateUserProfileProperty(userName, context) {
        let userProfile = await this.userProfileAccessor.get(context);
        if (userProfile === undefined) {
            userProfile = new UserProfile(userName);
        } else {
            userProfile.userName = userName;
        }
        return await this.userProfileAccessor.set(context, userProfile);
    }
};
