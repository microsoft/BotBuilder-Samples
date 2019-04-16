// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const fetch = require('node-fetch');
const { ActivityHandler, ActivityTypes } = require('botbuilder');
const { MicrosoftTranslator } = require('./microsoftTranslator');
const { TranslationSettings } = require('./translationSettings');

const englishEnglish = TranslationSettings.englishEnglish;
const englishSpanish = TranslationSettings.englishSpanish;
const DEFAULT_LANGUAGE = englishEnglish;

class TranslatorMiddleware extends ActivityHandler {
    /**
     * Creates a translation middleware.
     * @param {BotStatePropertyAccessor} languagePreferenceProperty Accessor for language preference property in the user state.
     * @param {string} translatorKey Microsoft Text Translation API key.
     */
    constructor(languagePreferenceProperty, translatorKey, userState) {
        super();
        if (!languagePreferenceProperty) throw new Error('[TranslatorMiddleware]: Missing parameter. languagePreferenceProperty is required');
        if (!translatorKey) throw new Error('[TranslatorMiddleware]: Missing parameter. translatorKey is required');

        this.languagePreferenceProperty = languagePreferenceProperty;
        this.translatorKey = translatorKey;
        this.userState = userState;

        this.language = new MicrosoftTranslator();

        this.onTurn = this.onTurn.bind(this);
    }

    async onTurn(context, next) {
        if (context === null) {
            throw new Error(`Context is returning null`);
        };

        var translate = await this.shouldTranslateAsync(context);

        if (translate) {
            if (context.activity.type === ActivityTypes.Message) {
                context.activity.text = await this.translate(context.activity.text, DEFAULT_LANGUAGE);
            }
        };

        await context.onSendActivities(async (context, activities, next) => {
            // Translate messages sent to the user to user language
            const userLanguage = await this.languagePreferenceProperty.get(context, () => DEFAULT_LANGUAGE);
            const shouldTranslate = userLanguage !== DEFAULT_LANGUAGE;

            if (shouldTranslate) {
                for (const activity of activities) {
                    if (activity.type === ActivityTypes.Message) {
                        activity.text = await this.translate(activity.text, userLanguage);
                        // await this.translateMessageActivity(activity, userLanguage);
                    }
                }
            }
            await next();
        });

        await this.onConversationUpdate(async (context, activity, next) => {
            const userLanguage = await this.languagePreferenceProperty.get(context, () => DEFAULT_LANGUAGE);
            const shouldTranslate = userLanguage !== DEFAULT_LANGUAGE;

            if (activity.type === ActivityTypes.Message) {
                if (shouldTranslate) {
                    activity.text = await this.translate(activity.text, userLanguage);
                    // await this.translateMessageActivity(activity, userLanguage);
                }
            }
            await next();
        });

        // By calling next() you ensure that the next BotHandler is run.
        await next();
    };

    async translateMessageActivity(activity, targetLocale) {
        if (activity.type === ActivityTypes.Message) {
            activity.text = await this.translate(activity.text, targetLocale);
        }
    }

    async shouldTranslateAsync(context) {
        const userLanguage = await this.languagePreferenceProperty.get(context, DEFAULT_LANGUAGE);
        return userLanguage !== DEFAULT_LANGUAGE;
    };

    async translate(text, targetLocale) {
        // Check to make sure "en" is not translated to "in", or "es" to "it"
        // In a production bot scenario, this would be replaced for a method call that detects
        // language names in utterances.
        if (text.toLowerCase() === englishEnglish || text.toLowerCase() === englishSpanish) {
            return text;
        }

        // From Microsoft Text Translator API docs;
        // https://docs.microsoft.com/en-us/azure/cognitive-services/translator/quickstart-nodejs-translate
        const host = 'https://api.cognitive.microsofttranslator.com';
        const path = '/translate?api-version=3.0';
        const params = '&to=';

        const url = host + path + params + targetLocale;

        return fetch(url, {
            method: 'POST',
            body: JSON.stringify([{ 'Text': text }]),
            headers: {
                'Content-Type': 'application/json',
                'Ocp-Apim-Subscription-Key': this.translatorKey
            }
        })
            .then(res => res.json())
            .then(responseBody => {
                if (responseBody && responseBody.length > 0) {
                    return responseBody[0].translations[0].text;
                } else {
                    return text;
                }
            });
    }
}

module.exports.TranslatorMiddleware = TranslatorMiddleware;
