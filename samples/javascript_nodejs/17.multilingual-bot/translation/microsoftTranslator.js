// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const fetch = require('node-fetch');

class MicrosoftTranslator {
    constructor(translatorKey) {
        this.key = translatorKey;
    }
    /**
   * Helper method to translate text to a specified language.
   * @param {string} text Text that will be translated
   * @param {string} targetLocale Two character language code, e.g. "en", "es"
   */
    async translate(text, targetLocale) {
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
                'Ocp-Apim-Subscription-Key': this.key
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
