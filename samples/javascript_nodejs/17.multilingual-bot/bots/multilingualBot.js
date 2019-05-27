// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { ActivityHandler, ActionTypes, CardFactory, MessageFactory } = require('botbuilder');
const { TranslationSettings } = require('../translation/translationSettings');

const englishEnglish = TranslationSettings.englishEnglish;
const englishSpanish = TranslationSettings.englishSpanish;
const spanishEnglish = TranslationSettings.spanishEnglish;
const spanishSpanish = TranslationSettings.spanishSpanish;

const WelcomeCard = require('../cards/welcomeCard.json');

/**
 * A simple bot that captures user preferred language and uses state to configure
 * user's language preference so that middleware translates accordingly when needed.
 */
class MultilingualBot extends ActivityHandler {
    /**
     * Creates a Multilingual bot.
     * @param {Object} userState User state object.
     * @param {Object} languagePreferenceProperty Accessor for language preference property in the user state.
     * @param {any} logger object for logging events, defaults to console if none is provided
     */
    constructor(userState, languagePreferenceProperty, logger) {
        super();
        if (!userState) {
            throw new Error('[MultilingualBot]: Missing parameter. userState is required');
        }
        if (!languagePreferenceProperty) {
            throw new Error('[MultilingualBot]: Missing parameter. languagePreferenceProperty is required');
        }
        if (!logger) {
            logger = console;
            logger.log('[MultilingualBot]: logger not passed in, defaulting to console');
        }

        this.userState = userState;
        this.languagePreferenceProperty = languagePreferenceProperty;
        this.logger = logger;

        this.onMembersAdded(async (context, next) => {
            const membersAdded = context.activity.membersAdded;
            for (let cnt = 0; cnt < membersAdded.length; cnt++) {
                if (membersAdded[cnt].id !== context.activity.recipient.id) {
                    const welcomeCard = CardFactory.adaptiveCard(WelcomeCard);
                    await context.sendActivity({ attachments: [welcomeCard] });
                    await context.sendActivity(`This bot will introduce you to translation middleware. Say 'hi' to get started.`);
                }
            }

            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });

        this.onMessage(async (context, next) => {
            this.logger.log('Running dialog with Message Activity.');

            if (isLanguageChangeRequested(context.activity.text)) {
                const currentLang = context.activity.text.toLowerCase();
                const lang = currentLang === englishEnglish || currentLang === spanishEnglish ? englishEnglish : englishSpanish;

                // Get the user language preference from the user state.
                await this.languagePreferenceProperty.set(context, lang);

                // If the user requested a language change through the suggested actions with values "es" or "en",
                // simply change the user's language preference in the user state.
                // The translation middleware will catch this setting and translate both ways to the user's
                // selected language.
                // If Spanish was selected by the user, the reply below will actually be shown in spanish to the user.
                await context.sendActivity(`Your current language code is: ${ lang }`);
                await this.userState.saveChanges(context);
            } else {

                await context.sendActivity(context.activity.text);

                // Show the user the possible options for language. The translation middleware
                // will pick up the language selected by the user and
                // translate messages both ways, i.e. user to bot and bot to user.
                // Create an array with the supported languages.
                const cardActions = [
                    {
                        type: ActionTypes.PostBack,
                        title: 'Español',
                        value: englishSpanish
                    },
                    {
                        type: ActionTypes.PostBack,
                        title: 'English',
                        value: englishEnglish
                    }
                ];
                const reply = MessageFactory.suggestedActions(cardActions, `Choose your language:`);
                await context.sendActivity(reply);
            }

            // By calling next() you ensure that the next BotHandler is run.
            await next();
        });
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
 */
function isLanguageChangeRequested(utterance) {
    // If the utterance is empty or the utterance is not a supported language code,
    // then there is no language change requested
    if (!utterance) {
        return false;
    }

    // We know that the utterance is a language code. If the code sent in the utterance
    // is different from the current language, then a change was indeed requested
    utterance = utterance.toLowerCase().trim();
    return utterance === englishSpanish || utterance === englishEnglish || utterance === spanishSpanish || utterance === spanishEnglish;
}

module.exports.MultilingualBot = MultilingualBot;
