// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

const assert = require('assert');
const {MicrosoftTranslator, TranslateArrayOptions} = require('../translator');
const translatorKey = process.env.translatorKey;

describe('MicrosoftTranslator', function () {
    this.timeout(20000);
    
    if (!translatorKey) 
    {
        console.warn('WARNING: skipping MicrosoftTranslator test suite because TRANSLATORKEY environment letiable is not defined');
        return;
    }

    it('should translate en to fr and support html tags in sentences', async()=> {
        
        let translationOptions = new TranslateArrayOptions();
        translationOptions.from = "en";
        translationOptions.to = "fr";
        translationOptions.texts = ["greetings <br> My friends"];

        let translator = new MicrosoftTranslator(translatorKey);
        const translationResult = await translator.translateArray(translationOptions);
        assert.equal(translationResult[0].translatedText, "salutations <br> mes amis");
    });

    it('should handle punctuations', async()=> {
        
        let translationOptions = new TranslateArrayOptions();
        translationOptions.from = "en";
        translationOptions.to = "fr";
        translationOptions.texts = ['0: You said "hello"'];

        let translator = new MicrosoftTranslator(translatorKey);
        const translationResult = await translator.translateArray(translationOptions);
        assert.equal(translationResult[0].translatedText, '0 : vous avez dit " Bonjour "');
    });

    it('should not translate no translate texts and numbers', async()=> {
        
        let translationOptions = new TranslateArrayOptions();
        translationOptions.from = "fr";
        translationOptions.to = "en";
        translationOptions.texts = ['Bonjour Jean mon ami 2018'];
        let noTranslatePatterns = {
                                    "fr" : ['Bonjour (Jean mon ami)']
                                };
        let translator = new MicrosoftTranslator(translatorKey, noTranslatePatterns);
        const translationResult = await translator.translateArray(translationOptions);
        assert.equal(translationResult[0].translatedText, 'Hello Jean mon ami 2018');
    });

    it('should handle empty messages', async()=> {
        
        let translationOptions = new TranslateArrayOptions();
        translationOptions.from = "fr";
        translationOptions.to = "en";
        translationOptions.texts = ['\n\n'];
        let translator = new MicrosoftTranslator(translatorKey);

        const translationResult = await translator.translateArray(translationOptions);
        assert.equal(translationResult[0].translatedText, '');
    });

    it('should handle no translate texts with no groups', async()=> {
        
        let translationOptions = new TranslateArrayOptions();
        translationOptions.from = "fr";
        translationOptions.to = "en";
        translationOptions.texts = ['Bonjour Jean mon ami'];
        let noTranslatePatterns = { "fr" : ['Jean mon ami'] };
        let translator = new MicrosoftTranslator(translatorKey, noTranslatePatterns);

        const translationResult = await translator.translateArray(translationOptions);
        assert.equal(translationResult[0].translatedText, 'Hello Jean mon ami');
    });
    
    it('should handle special cases in no translates - 1', async()=> {
        
        let translationOptions = new TranslateArrayOptions();
        translationOptions.from = "es";
        translationOptions.to = "en";
        translationOptions.texts = ['mi perro se llama Enzo'];
        let noTranslatePatterns = { "es" : ['perr[oa]', 'Hi']};
        let translator = new MicrosoftTranslator(translatorKey, noTranslatePatterns);

        const translationResult = await translator.translateArray(translationOptions);
        assert.equal(translationResult[0].translatedText, "My perro's name is Enzo");
    });
    
    it('should handle special cases in no translates - 2', async()=> {
        
        let translationOptions = new TranslateArrayOptions();
        translationOptions.from = "fr";
        translationOptions.to = "en";
        translationOptions.texts = ["mon nom est l'etat"];
        let noTranslatePatterns = { "fr" : ['mon nom est (.+)']};
        let translator = new MicrosoftTranslator(translatorKey, noTranslatePatterns);

        const translationResult = await translator.translateArray(translationOptions);
        assert.equal(translationResult[0].translatedText, "My name is l'etat");
    });

    it('should use dictionary', async()=> {
        
        let translationOptions = new TranslateArrayOptions();
        translationOptions.from = "en";
        translationOptions.to = "fr";
        translationOptions.texts = ["I want to stay in a royal room"];
        let wordDictionary = { 'room': 'lieu', 'royal': 'régalien' };
        let translator = new MicrosoftTranslator(translatorKey, null, wordDictionary);
        const translationResult = await translator.translateArray(translationOptions);
        assert.equal(translationResult[0].translatedText, "Je veux rester dans une lieu régalien");
    });

    it('should detect language', async()=> {
        let translator = new MicrosoftTranslator(translatorKey);
        const detectedLanguage = await translator.detect("hello world");
        assert.equal(detectedLanguage[0].language, "en");
    });
})
