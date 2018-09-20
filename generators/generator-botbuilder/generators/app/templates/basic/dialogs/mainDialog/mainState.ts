// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.

// This file defines data model for the bot.

const TURN_COUNTER = 'turnCounter';

class TurnCounter {
    /**
     *
     * @param {object} state state instance - can be user or conversation state.
     */
    constructor(state) {
        if (!state || !state.createProperty) throw ('Invalid state provided. Need either converesation or user state');

        // creates a new state accessor property.see https://aka.ms/about-bot-state-accessors to learn more about the bot state and state accessors
        this.countProperty = state.createProperty(TURN_COUNTER);
    }

    /**
     *
     * @param {object} context context object
     */
    async get(context) {
        if (!context) throw ('Invalid context provided');
        return await this.countProperty.get(context);
    }
    /**
     *
     * @param {object} context context object
     * @param {number} value new count value to set
     */
    async set(context, value) {
        if (!context) throw ('Invalid context provided');

        // perform any (optional) validations on value before grounding
        if (isNaN(value)) throw ('Can only take numbers for TurnCounter state')
        if (!value || value < 0) throw ('Invalid value for count');
        return this.countProperty.set(context, value);
    }
}

module.exports = TurnCounter;