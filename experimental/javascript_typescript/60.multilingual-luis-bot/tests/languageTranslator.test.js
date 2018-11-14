// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const assert = require('assert');
const { MicrosoftTranslator, TranslateArrayOptions } = require('../src/translator');
const translatorKey = process.env.translatorKey;

describe('MicrosoftTranslator', function () {
    this.timeout(20000);

    if (!translatorKey) {
        console.warn('WARNING: skipping MicrosoftTranslator test suite because TRANSLATORKEY environment variable is not defined');
        return;
    }

    it('should translate en to fr and support html tags in sentences', async () => {
        const translationOptions = {
            from: "en",
            to: "fr",
            texts: ["greetings <br> My friends"]
        }
        const translator = new MicrosoftTranslator(translatorKey);
        const translationResult = await translator.translateArray(translationOptions);
        assert.equal(translationResult[0].translatedText, "salutations <br> mes amis");
    });

    it('should handle punctuations', async () => {
        const translationOptions = {
            from: "en",
            to: "fr",
            texts: ['0: You said "hello"']
        }
        const translator = new MicrosoftTranslator(translatorKey);
        const translationResult = await translator.translateArray(translationOptions);
        assert.equal(translationResult[0].translatedText, '0 : vous avez dit " Bonjour "');
    });

    it('should not translate no translate texts and numbers', async () => {
        const translationOptions = {
            from: "fr",
            to: "en",
            texts: ['Bonjour Jean mon ami 2018']
        }
        const noTranslatePatterns = {
            "fr": ['Bonjour (Jean mon ami)']
        };
        const translator = new MicrosoftTranslator(translatorKey, noTranslatePatterns);
        const translationResult = await translator.translateArray(translationOptions);
        assert.equal(translationResult[0].translatedText, 'Hello Jean mon ami 2018');
    });

    it('should handle empty messages', async () => {
        const translationOptions = {
            from: "fr",
            to: "en",
            texts: ['\n\n']
        }
        const translator = new MicrosoftTranslator(translatorKey);
        const translationResult = await translator.translateArray(translationOptions);
        assert.equal(translationResult[0].translatedText, '');
    });

    it('should handle no translate texts with no groups', async () => {
        const translationOptions = {
            from: "fr",
            to: "en",
            texts: ['Bonjour Jean mon ami']
        }
        const noTranslatePatterns = { "fr": ['Jean mon ami'] };
        const translator = new MicrosoftTranslator(translatorKey, noTranslatePatterns);
        const translationResult = await translator.translateArray(translationOptions);
        assert.equal(translationResult[0].translatedText, 'Hello Jean mon ami');
    });

    it('should handle special cases in no translates - 1', async () => {
        const translationOptions = {
            from: "es",
            to: "en",
            texts: ['mi perro se llama Enzo']
        }
        const noTranslatePatterns = { "es": ['perr[oa]', 'Hi'] };
        const translator = new MicrosoftTranslator(translatorKey, noTranslatePatterns);
        const translationResult = await translator.translateArray(translationOptions);
        assert.equal(translationResult[0].translatedText, "My perro's name is Enzo");
    });

    it('should handle special cases in no translates - 2', async () => {
        const translationOptions = {
            from: "fr",
            to: "en",
            texts: ["mon nom est l'etat"]
        }
        const noTranslatePatterns = { "fr": ['mon nom est (.+)'] };
        const translator = new MicrosoftTranslator(translatorKey, noTranslatePatterns);
        const translationResult = await translator.translateArray(translationOptions);
        assert.equal(translationResult[0].translatedText, "My name is l'etat");
    });

    it('should use dictionary', async () => {
        const translationOptions = {
            from: "en",
            to: "fr",
            texts: ["I want to stay in a royal room"]
        }
        const wordDictionary = { 'room': 'lieu', 'royal': 'régalien' };
        const translator = new MicrosoftTranslator(translatorKey, null, wordDictionary);
        const translationResult = await translator.translateArray(translationOptions);
        assert.equal(translationResult[0].translatedText, "Je veux rester dans une lieu régalien");
    });

    it('should detect language', async () => {
        const translator = new MicrosoftTranslator(translatorKey);
        const detectedLanguage = await translator.detect("hello world");
        assert.equal(detectedLanguage[0].language, "en");
    });
})
