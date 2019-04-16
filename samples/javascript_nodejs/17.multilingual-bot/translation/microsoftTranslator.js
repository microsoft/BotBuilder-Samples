// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const fetch = require('node-fetch');
const { TranslationSettings } = require('./translationSettings');

const englishEnglish = TranslationSettings.englishEnglish;
const englishSpanish = TranslationSettings.englishSpanish;

class MicrosoftTranslator {
    constructor() {
        this.translate = this.translate.bind(this);
    }
    /**
   * Helper method to translate text to a specified language.
   * @param {string} text Text that will be translated
   * @param {string} targetLocale Two character language code, e.g. "en", "es"
   */
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

module.exports.MicrosoftTranslator = MicrosoftTranslator;
