"use strict";
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
var __awaiter = (this && this.__awaiter) || function (thisArg, _arguments, P, generator) {
    return new (P || (P = Promise))(function (resolve, reject) {
        function fulfilled(value) { try { step(generator.next(value)); } catch (e) { reject(e); } }
        function rejected(value) { try { step(generator["throw"](value)); } catch (e) { reject(e); } }
        function step(result) { result.done ? resolve(result.value) : new P(function (resolve) { resolve(result.value); }).then(fulfilled, rejected); }
        step((generator = generator.apply(thisArg, _arguments || [])).next());
    });
};
Object.defineProperty(exports, "__esModule", { value: true });
// bot.js is your bot's main entry point to handle incoming activities.
const botbuilder_1 = require("botbuilder");
// Turn counter property
const TURN_COUNTER = 'turnCounterProperty';
class EchoBot {
    constructor(conversationState) {
        // creates a new state accessor property.see https://aka.ms/about-bot-state-accessors to learn more about the bot state and state accessors 
        this.countAccessor = conversationState.createProperty(TURN_COUNTER);
        this.conversationState = conversationState;
    }
    /**
     *
     * Use onTurn to handle an incoming activity, received from a user, process it, and reply as needed
     *
     * @param {TurnContext} context on turn context object.
     */
    onTurn(turnContext) {
        return __awaiter(this, void 0, void 0, function* () {
            // Handle message activity type. User's responses via text or speech or card interactions flow back to the bot as Message activity.
            // Message activities may contain text, speech, interactive cards, and binary or unknown attachments.
            // see https://aka.ms/about-bot-activity-message to learn more about the message and other activity types
            if (turnContext.activity.type === botbuilder_1.ActivityTypes.Message) {
                // read from state.
                let count = yield this.countAccessor.get(turnContext);
                count = count === undefined ? 1 : ++count;
                yield turnContext.sendActivity(`${count}: You said "${turnContext.activity.text}"`);
                // increment and set turn counter.
                yield this.countAccessor.set(turnContext, count);
            }
            else {
                // Generic handler for all other activity types.
                yield turnContext.sendActivity(`[${turnContext.activity.type} event detected]`);
            }
            // Save state changes
            yield this.conversationState.saveChanges(turnContext);
        });
    }
}
exports.EchoBot = EchoBot;
//# sourceMappingURL=bot.js.map