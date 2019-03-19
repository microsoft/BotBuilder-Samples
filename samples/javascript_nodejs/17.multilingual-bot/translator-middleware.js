// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const fetch = require('node-fetch');

const ENGLISH_LANGUAGE = 'en';
const SPANISH_LANGUAGE = 'es';
const DEFAULT_LANGUAGE = ENGLISH_LANGUAGE;

class TranslatorMiddleware {
    /**
     * Creates a translation middleware.
     * @param {BotStatePropertyAccessor} languagePreferenceProperty Accessor for language preference property in the user state.
     * @param {string} translatorKey Microsoft Text Translation API key.
     */
    constructor(languagePreferenceProperty, translatorKey) {
        this.languagePreferenceProperty = languagePreferenceProperty;
        this.translatorKey = translatorKey;
    }

    /**
     * Called via BotAdapter.runMiddleware(), this method adds handlers to the TurnContext's sendActivities method to
     * translate the outgoing text.
     * @param {TurnContext} turnContext A TurnContext instance containing all the data needed for processing this conversation turn.
     * @param {Function} next The next middleware or business logic of the bot to run.
     */
    async onTurn(turnContext, next) {
        if (turnContext.activity.type === 'message') {
            const userLanguage = await this.languagePreferenceProperty.get(turnContext, DEFAULT_LANGUAGE);
            const shouldTranslate = userLanguage !== DEFAULT_LANGUAGE;

            if (shouldTranslate) {
                turnContext.activity.text = await this.translate(turnContext.activity.text, DEFAULT_LANGUAGE);
            }

            turnContext.onSendActivities(async (context, activities, next) => {
                // Translate messages sent to the user to user language
                const userLanguage = await this.languagePreferenceProperty.get(turnContext, DEFAULT_LANGUAGE);
                const shouldTranslate = userLanguage !== DEFAULT_LANGUAGE;

                if (shouldTranslate) {
                    for (const activity of activities) {
                        activity.text = await this.translate(activity.text, userLanguage);
                    }
                }
                await next();
            });
        }
        await next();
    }

    /**
     * Helper method to translate text to a specified language.
     * @param {string} text Text that will be translated
     * @param {string} to Two character langauge code, e.g. "en", "es"
     */
    async translate(text, to) {
        // Check to make sure "en" is not translated to "in", or "es" to "it"
        // In a production bot scenario, this would be replaced for a method call that detects
        // language names in utterances.
        if (text.toLowerCase() === ENGLISH_LANGUAGE || text.toLowerCase() === SPANISH_LANGUAGE) {
            return text;
        }

        // From Microsoft Text Translator API docs;
        // https://docs.microsoft.com/en-us/azure/cognitive-services/translator/quickstart-nodejs-translate
        const host = 'https://api.cognitive.microsofttranslator.com';
        const path = '/translate?api-version=3.0';
        const params = '&to=';

        const url = host + path + params + to;

        return fetch(url, {
            method: 'post',
            body: JSON.stringify([{ 'Text': text }]),
            headers: {
                'Content-Type': 'application/json',
                'Ocp-Apim-Subscription-Key': this.translatorKey
            }
        })
            .then(res => res.json())
            .then(jsonResponse => {
                if (jsonResponse && jsonResponse.length > 0) {
                    return jsonResponse[0].translations[0].text;
                } else {
                    return text;
                }
            });
    }
}

module.exports.TranslatorMiddleware = TranslatorMiddleware;
