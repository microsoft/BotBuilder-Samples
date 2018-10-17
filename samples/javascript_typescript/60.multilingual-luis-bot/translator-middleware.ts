// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

"use strict";
Object.defineProperty(exports, "__esModule", { value: true });

import { Middleware, TurnContext, ConversationState, BotStatePropertyAccessor }  from 'botbuilder';
import {MicrosoftTranslator, TranslateArrayOptions} from './translator';
let _translator = null;

const ENGLISH_LANGUAGE = 'en';
const DEFAULT_LANGUAGE = ENGLISH_LANGUAGE;
export class TranslatorMiddleware {
    private translatorKey: string;
    private to: string;
    /**
     * Creates a translation middleware.
    *
        @param {string} translatorKey Microsoft Text Translation API key.
    *   @param {string} to 
     */
    constructor(to, translatorKey, noTranslatePatterns?: {[id: string] : string[]}, wordDictionary?: { [id: string]: string }) {
        this.translatorKey = translatorKey;
        _translator = new MicrosoftTranslator(translatorKey, noTranslatePatterns, wordDictionary);
        this.to = to;
    }

    /**
     * Called via BotAdapter.runMiddleware(), this method adds handlers to the TurnContext's sendActivities method to
     * translate the outgoing text.
     * @param {TurnContext} turnContext A TurnContext instance containing all the data needed for processing this conversation turn.
     * @param {Function} next The next middleware or business logic of the bot to run.
     */
    async onTurn(turnContext, next) {
        if (turnContext.activity.type === 'message') {
            const detectedLanguageId = (await _translator.detect(turnContext.activity.text))[0].language;
            const shouldTranslate = detectedLanguageId !== this.to;

            if (shouldTranslate) {
                var options = new TranslateArrayOptions();
                options.from = detectedLanguageId;
                options.to = this.to;
                options.texts = [turnContext.activity.text];    
                turnContext.activity.text = (await _translator.translateArray(options))[0].translatedText;
            }
        }
        await next();
    }
}

module.exports.TranslatorMiddleware = TranslatorMiddleware;
