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
// Turn counter property
const TURN_COUNTER = 'turnCounter';
class MainDialog {
    constructor(conversationState) {
        // creates a new state accessor property.see https://aka.ms/about-bot-state-accessors to learn more about the bot state and state accessors 
        this.countProperty = conversationState.createProperty(TURN_COUNTER);
    }
    /**
     *
     * @param {Object} context on turn context object.
     */
    onTurn(context) {
        return __awaiter(this, void 0, void 0, function* () {
            // see https://aka.ms/about-bot-activity-message to learn more about the message and other activity types
            if (context.activity.type === 'message') {
                // read from state.
                let count = yield this.countProperty.get(context);
                count = count === undefined ? 1 : count;
                yield context.sendActivity(`${count}: You said "${context.activity.text}"`);
                // increment and set turn counter.
                this.countProperty.set(context, ++count);
            }
            else {
                yield context.sendActivity(`[${context.activity.type} event detected]`);
            }
        });
    }
}
exports.MainDialog = MainDialog;
//# sourceMappingURL=index.js.map