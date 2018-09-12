// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const { Middleware, TurnContext, ConversationState } = require('botbuilder');
const fetch = require('node-fetch');

const LANGUAGE_PREFERENCE = 'language_preference';
const ENGLISH_LANGUAGE = 'en';
const SPANISH_LANGUAGE = 'es';
const DEFAULT_LANGUAGE = ENGLISH_LANGUAGE;

class TranslatorMiddleware {

    /**
     * Creates a translation middleware
     * @param {StatePropertyAccessor} languagePreferenceProperty Accessor for language preference property in the user state.
     * * @param {string} translatorKey Microsoft Text Translation API key.
     */
    constructor(languagePreferenceProperty, translatorKey) { 
        
        this.languagePreferenceProperty = languagePreferenceProperty;
        this.translatorKey = translatorKey;
    }

    async onTurn(turnContext, next) {
        if (turnContext.activity.type === 'message') {

            const userLanguage = await this.languagePreferenceProperty.get(turnContext, DEFAULT_LANGUAGE);
            const shouldTranslate = userLanguage != DEFAULT_LANGUAGE;

            if(shouldTranslate) {
                turnContext.activity.text = await this.translate(turnContext.activity.text, DEFAULT_LANGUAGE);
            }

            turnContext.onSendActivities(async (context, activities, next) => {
                // Translate messages sent to the user to user language
                const userLanguage = await this.languagePreferenceProperty.get(turnContext, DEFAULT_LANGUAGE);
                const shouldTranslate = userLanguage != DEFAULT_LANGUAGE;
    
                if(shouldTranslate) {
                    for(const activity of activities) {
                        activity.text = await this.translate(activity.text, userLanguage);
                    }
                }
                await next();
            });

            turnContext.onUpdateActivity(async (context, activitiy, next) => {
                // Translate messages sent to the user to user language
                const userLanguage = await this.languagePreferenceProperty.get(turnContext, DEFAULT_LANGUAGE);
                const shouldTranslate = userLanguage != DEFAULT_LANGUAGE;
    
                if(shouldTranslate) {
                    activity.text = await this.translate(turnContext.activity.text, userLanguage);
                }
                await next();
            });
        }
        await next();
    }

    async translate(text, to) {
        // From Microsoft Text Translator API docs;
        // https://docs.microsoft.com/en-us/azure/cognitive-services/translator/quickstart-nodejs-transliterate
        const host = 'https://api.cognitive.microsofttranslator.com';
        const path = '/translate?api-version=3.0';
        const params = '&to=';

        const url = host + path + params + to;

        return fetch(url, { 
                method: 'post',
                body:    JSON.stringify ([{'Text' : text}]),
                headers: {
                    'Content-Type' : 'application/json',
                    'Ocp-Apim-Subscription-Key' : this.translatorKey
                },
            })
            .then(res => res.json())
            .then(jsonResponse => {
                if(jsonResponse && jsonResponse.length > 0) {
                    return jsonResponse[0].translations[0].text;
                } else {
                    return text;
                }
                
            });
    }
}

module.exports = TranslatorMiddleware;