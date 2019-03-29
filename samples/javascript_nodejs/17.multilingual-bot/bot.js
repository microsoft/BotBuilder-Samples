// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityTypes, MessageFactory } = require('botbuilder');

const ENGLISH_LANGUAGE = 'en';
const SPANISH_LANGUAGE = 'es';
const DEFAULT_LANGUAGE = ENGLISH_LANGUAGE;

/**
 * A simple bot that captures user preferred language and uses state to configure
 * user's language preference so that middleware translates accordingly when needed.
 */
class MultilingualBot {
    /**
     * Creates a Multilingual bot.
     * @param {Object} userState User state object.
     * @param {Object} languagePreferenceProperty Accessor for language preference property in the user state.
     */
    constructor(userState, languagePreferenceProperty) {
        this.languagePreferenceProperty = languagePreferenceProperty;
        this.userState = userState;
    }

    /**
     * Every conversation turn for our MultilingualBot will call this method.
     * There are no dialogs used, since it's "single turn" processing, meaning a single request and
     * response, with no stateful conversation.
     * @param {Object} turnContext A TurnContext instance containing all the data needed for processing this conversation turn.
     */
    async onTurn(turnContext) {
        // See https://aka.ms/about-bot-activity-message to learn more about the message and other activity types.
        if (turnContext.activity.type === ActivityTypes.Message) {
            const text = turnContext.activity.text;

            // Get the user language preference from the user state.
            const userLanguage = await this.languagePreferenceProperty.get(turnContext, DEFAULT_LANGUAGE);

            if (isLanguageChangeRequested(text, userLanguage)) {
                // If the user requested a language change through the suggested actions with values "es" or "en",
                // simply change the user's language preference in the user state.
                // The translation middleware will catch this setting and translate both ways to the user's
                // selected language.
                // If Spanish was selected by the user, the reply below will actually be shown in spanish to the user.
                await this.languagePreferenceProperty.set(turnContext, text);
                await turnContext.sendActivity(`Your current language code is: ${ text }`);
            } else {
                // Show the user the possible options for language. The translation middleware
                // will pick up the language selected by the user and
                // translate messages both ways, i.e. user to bot and bot to user.
                // Create an array with the supported languages.
                const reply = MessageFactory.suggestedActions([SPANISH_LANGUAGE, ENGLISH_LANGUAGE], `Choose your language:`);
                await turnContext.sendActivity(reply);
            }
            await this.userState.saveChanges(turnContext);
        }
    }
}

/**
 * Checks whether the utterance from the user is requesting a language change.
 * In a production bot, we would use the Microsoft Text Translation API language
 * detection feature, along with detecting language names.
 * For the purpose of the sample, we just assume that the user requests language
 * changes by responding with the language code through the suggested action presented
 * above or by typing it.
 * @param {string} utterance the current turn utterance.
 * @param {string} currentLanguage the current user language.
 */
function isLanguageChangeRequested(utterance, currentLanguage) {
    // If the utterance is empty or the utterance is not a supported language code,
    // then there is no language change requested
    if (!utterance) {
        return false;
    }

    const cleanedUpUtterance = utterance.toLowerCase().trim();

    if (!isSupportedLanguageCode(cleanedUpUtterance)) {
        return false;
    }

    // We know that the utterance is a language code. If the code sent in the utterance
    // is different from the current language, then a change was indeed requested
    return cleanedUpUtterance !== currentLanguage;
}

/**
 * Checks whether the utterance from the user is one of the 2 supported language codes in this sample
 * @param {string} utterance the current turn utterance.
 */
function isSupportedLanguageCode(utterance) {
    if (!utterance) {
        return false;
    }
    return utterance === SPANISH_LANGUAGE || utterance === ENGLISH_LANGUAGE;
}

module.exports.MultilingualBot = MultilingualBot;
